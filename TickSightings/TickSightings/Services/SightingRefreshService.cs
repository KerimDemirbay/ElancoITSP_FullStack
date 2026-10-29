using TickSightings.Models;

namespace TickSightings.Services;

public class SightingRefreshService(
//this service is a singleton, so it must not hold a short-lived HttpClient
//factory decides when to drop client, and make a new one
    IServiceScopeFactory scopeFactory,
//store results
    SightingStore store,
//appsettings
    IConfiguration configuration,
//writes to console
    ILogger<SightingRefreshService> logger
    ) : BackgroundService //runs for the apps entire life
{
    private static readonly TimeSpan RetryWhenEmpty = TimeSpan.FromMinutes(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var refreshInterval = configuration.GetValue("ElancoApi:RefreshInterval", TimeSpan.FromMinutes(30));

        while (!stoppingToken.IsCancellationRequested)
        {
            await RefreshAsync(stoppingToken);

            var delay = store.Current is null ? RetryWhenEmpty : refreshInterval;
        //as to not stop a thread. Throws an expected OperationCancelled exeception and ends waiting loop cleanly
            await Task.Delay(delay, stoppingToken);
        }
    }

    private async Task RefreshAsync(CancellationToken stoppingToken)
    {
        try
        {
        //new scope per refresh = fresh ElancoApiClient from the factory
        //scope disposed when the method ends
            using var scope = scopeFactory.CreateScope();
            var elanco = scope.ServiceProvider.GetRequiredService<ElancoApiClient>();

            var raw = await elanco.GetAllSightingsAsync(stoppingToken);
            var result = SightingCleaner.Clean(raw, DateTime.Now);

            store.ReplaceData(new DataSnapshot(result.Sightings, result.Report, DateTimeOffset.UtcNow));

            logger.LogInformation("Refreshed sightings from Elanco: {Accepted} of {Received} accepted",
                result.Report.Accepted, result.Report.Received);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Thrown exception when the app is shutdown. 
        }
        catch (Exception ex)
        {
            store.RecordFailure(new RefreshFailure(DateTimeOffset.UtcNow, ex.Message));
            logger.LogError(ex, "Refreshing sightings from Elanco failed. Keeping the previous data");
        }
    }
}