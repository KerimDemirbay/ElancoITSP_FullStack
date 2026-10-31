namespace TickSightings.Models;

public record SightingSearchResponse(
    IReadOnlyList<Sighting> Sightings,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages
);