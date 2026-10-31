using System.Globalization;
using Microsoft.EntityFrameworkCore;
using TickSightings.Data;
using TickSightings.Models;

namespace TickSightings.Services;

public class ReportService(SightingQueryService queries)
{
    public async Task<IReadOnlyList<RegionCount>> CountByRegionAsync(SightingFilter filter, CancellationToken cancellationToken)
    {
        var counts = await queries.Query(filter)
            .GroupBy(r => r.Region)
            .Select(g => new { Region = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Region, x => x.Count, cancellationToken);

        return UkCities.Regions
            .Select(region => new RegionCount(region, counts.GetValueOrDefault(region)))
            .OrderByDescending(r => r.Count)
            .ThenBy(r => r.Region)
            .ToList();
    }

    public async Task<IReadOnlyList<CityReport>> CountByCityAsync(SightingFilter filter, CancellationToken cancellationToken)
    {
        var rows = await queries.Query(filter)
            .GroupBy(s => new { s.City, s.Species })
            .Select(g => new { g.Key.City, g.Key.Species, Count = g.Count(), Latest = g.Max(s => s.Date) })
            .ToListAsync(cancellationToken);

        return UkCities.All
            .Select(city =>
            {
                var cityRows = rows.Where(r => r.City == city.Name).ToList();

                return new CityReport(
                    City: city.Name,
                    Region: city.Region,
                    Latitude: city.Latitude,
                    Longitude: city.Longitude,
                    Count: cityRows.Sum(r => r.Count),
                    LatestSighting: cityRows.Count == 0 ? null : cityRows.Max(r => r.Latest),
                    Species: cityRows
                        .OrderByDescending(r => r.Count)
                        .Select(r => new SpeciesCount(r.Species, r.Count))
                        .ToList());
            })
            .OrderByDescending(c => c.Count)
            .ToList();
    }

    public async Task<IReadOnlyList<TrendPoint>> TrendAsync(TrendQuery query, CancellationToken cancellationToken)
    {
        var dailyCounts = await queries.Query(query)
            .GroupBy(s => s.Date.Date)
            .Select(g => new { Day = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        if (dailyCounts.Count == 0)
            return [];

        var countsByPeriod = dailyCounts
            .GroupBy(d => PeriodStart(DateOnly.FromDateTime(d.Day), query.Interval))
            .ToDictionary(g => g.Key, g => g.Sum(d => d.Count));

        var first = countsByPeriod.Keys.Min();
        var last = countsByPeriod.Keys.Max();

        var points = new List<TrendPoint>();

        for (var period = first; period <= last; period = NextPeriod(period, query.Interval))
            points.Add(new TrendPoint(period, countsByPeriod.GetValueOrDefault(period)));

        return points;
    }

    public async Task<IReadOnlyList<MonthCount>> SeasonalAsync(SightingFilter filter, CancellationToken cancellationToken)
    {
        var counts = await queries.Query(filter)
            .GroupBy(s => s.Date.Month)
            .Select(g => new { Month = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Month, x => x.Count, cancellationToken);

        return Enumerable.Range(1, 12)
            .Select(month => new MonthCount(
                month,
                CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(month),
                SeasonOf(month),
                counts.GetValueOrDefault(month)))
            .ToList();
    }

    private static DateOnly PeriodStart(DateOnly day, TrendInterval interval)
    {
        if (interval == TrendInterval.Monthly)
        {
            return new DateOnly(day.Year, day.Month, 1);
        }

        var monday = day;
        while (monday.DayOfWeek != DayOfWeek.Monday)
        {
            monday = monday.AddDays(-1);
        }
        return monday;
    }

    private static DateOnly NextPeriod(DateOnly periodStart, TrendInterval interval)
    {
        if (interval == TrendInterval.Monthly)
        {
            return periodStart.AddMonths(1);
        }
        return periodStart.AddDays(7);
    }

    private static string SeasonOf(int month)
    {
        if (month == 12 || month <= 2) return "Winter";
        if (month <= 5) return "Spring";
        if (month <= 8) return "Summer";
        return "Autumn";
    }
}