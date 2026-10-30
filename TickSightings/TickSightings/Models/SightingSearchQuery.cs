using System.ComponentModel.DataAnnotations;

namespace TickSightings.Models;

public class SightingSearchQuery : SightingFilter
{
    [Range(1, int.MaxValue)]
    public int Page { get; init; } = 1;

    //limits query to 200 rows
    [Range(1, 200)]
    public int PageSize { get; init; } = 50;
}