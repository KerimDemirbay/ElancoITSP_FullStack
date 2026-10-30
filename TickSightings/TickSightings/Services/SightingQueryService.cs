using Microsoft.EntityFrameworkCore;
using TickSightings.Data;
using TickSightings.Database;
using TickSightings.Models;

namespace TickSightings.Services;

public class SightingQueryService(TickSightingsDbContext db)
{
    public Task<bool> HasAnyDataAsync(CancellationToken cancellationToken) =>
        db.Sightings.AnyAsync(cancellationToken);

    public Task<int> CountAllAsync(CancellationToken cancellationToken) =>
        db.Sightings.CountAsync(cancellationToken);

    public IQueryable<SightingEntity> Query(SightingFilter filter)
    {
        IQueryable<SightingEntity> query = db.Sightings.AsNoTracking();

        if (filter.From is not null)
        {
            var start = filter.From.Value.ToDateTime(TimeOnly.MinValue);
            query = query.Where(s => s.Date >= start);
        }

        if (filter.To is not null)
        {
            var endExclusive = filter.To.Value.AddDays(1).ToDateTime(TimeOnly.MinValue);
            query = query.Where(s => s.Date < endExclusive);
        }

        if (filter.City is not null)
        {
            var city = UkCities.Find(filter.City)!.Name;
            query = query.Where(s => s.City == city);
        }

        if (filter.Region is not null)
        {
            var region = UkCities.FindRegion(filter.Region)!;
            query = query.Where(s => s.Region == region);
        }

        if (filter.Species is not null)
        {
            var species = TickSpecies.Find(filter.Species)!.Name;
            query = query.Where(s => s.Species == species);
        }

        return query;
    }

    public async Task<(IReadOnlyList<Sighting> Items, int TotalCount)> SearchAsync(
        SightingSearchQuery search, CancellationToken cancellationToken)
    {
        var query = Query(search);

        var totalCount = await query.CountAsync(cancellationToken);

        var page = await query
            .OrderByDescending(s => s.Date)
            .ThenBy(s => s.Id)
            .Skip((search.Page - 1) * search.PageSize)
            .Take(search.PageSize)
            .ToListAsync(cancellationToken);

        return (page.Select(s => s.ToSighting()).ToList(), totalCount);
    }

    public async Task<Sighting?> FindByIdAsync(string id, CancellationToken cancellationToken)
    {
        var entity = await db.Sightings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        return entity?.ToSighting();
    }
}