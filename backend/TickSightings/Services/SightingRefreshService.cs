using Microsoft.EntityFrameworkCore;
using TickSightings.Database;
using TickSightings.Models;

namespace TickSightings.Services;

public class SightingRefreshService(
    IServiceScopeFactory scopeFactory,
    RefreshStatus status,
    IConfiguration configuration,
    ILogger<SightingRefreshService> logger) : BackgroundService
{
    private static readonly TimeSpan RetryAfterFailure = TimeSpan.FromMinutes(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var refreshInterval = configuration.GetValue("ElancoApi:RefreshInterval", TimeSpan.FromMinutes(30));

        while (!stoppingToken.IsCancellationRequested)
        {
            await RefreshAsync(stoppingToken);

            var delay = status.LastFailure is not null ? RetryAfterFailure : refreshInterval;
            await Task.Delay(delay, stoppingToken);
        }
    }

    private async Task RefreshAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var elanco = scope.ServiceProvider.GetRequiredService<ElancoApiClient>();
            var db = scope.ServiceProvider.GetRequiredService<TickSightingsDbContext>();

            var raw = await elanco.GetAllSightingsAsync(stoppingToken);
            var result = SightingCleaner.Clean(raw, DateTime.Now);

            if (result.Sightings.Count == 0)
                throw new InvalidOperationException("Elanco returned no valid sightings, so the existing data was kept.");

            await ReplaceElancoSightingsAsync(db, result.Sightings, stoppingToken);

            status.RecordSuccess(new RefreshSuccess(DateTimeOffset.UtcNow, result.Report));

            logger.LogInformation("Synced sightings from Elanco: {Accepted} of {Received} accepted",
                result.Report.Accepted, result.Report.Received);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // The app is shutting down. Not an error, so nothing to record.
        }
        catch (Exception ex)
        {
            status.RecordFailure(new RefreshFailure(DateTimeOffset.UtcNow, ex.Message));
            logger.LogError(ex, "Syncing sightings from Elanco failed. Keeping the existing data");
        }
    }

    private static async Task ReplaceElancoSightingsAsync(
        TickSightingsDbContext db, IReadOnlyList<Sighting> sightings, CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        await db.Sightings
            .Where(s => s.Source == "Elanco")
            .ExecuteDeleteAsync(cancellationToken);

        db.Sightings.AddRange(sightings.Select(SightingEntity.FromSighting));
        await db.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }
}