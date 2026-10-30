using System.Text.Json.Serialization;
namespace TickSightings.Models;

public record RegionCount(string Region, int Count);
public record SpeciesCount(string Species, int Count);

public record CityReport(
    string City,
    string Region,
    double Latitude,
    double Longitude,
    int Count,
    DateTime? LatestSighting,
    IReadOnlyList<SpeciesCount> Species
);

public record TrendPoint(DateOnly PeriodStart, int Count);
public record MonthCount(int Month, string MonthName, string Season, int Count);

[JsonConverter(typeof(JsonStringEnumConverter<TrendInterval>))]
public enum TrendInterval { 
    Weekly,
    Monthly,
}

public class TrendQuery : SightingFilter
{
    public TrendInterval Interval { get; init; } = TrendInterval.Monthly;
}