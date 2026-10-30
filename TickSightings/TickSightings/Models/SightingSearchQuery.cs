using System.ComponentModel.DataAnnotations;

namespace TickSightings.Models;

public class SightingSearchQuery : SightingFilter
{
    [Range(1, int.MaxValue)]
    public int Page { get; init; } = 1;

    [Range(1, 1_000_000)]
    public int PageSize { get; init; } = 1;
}