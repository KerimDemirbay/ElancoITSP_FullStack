using Microsoft.AspNetCore.Mvc;
using TickSightings.Models;
using TickSightings.Services;


namespace TickSightings.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController(SightingQueryService queries, ReportService reports) : ControllerBase
{
    [HttpGet("by-region")]
    public async Task<ActionResult<IReadOnlyList<RegionCount>>> ByRegion(
        [FromQuery] SightingFilter filter, CancellationToken cancellationToken)
    {
        if (!await queries.HasAnyDataAsync(cancellationToken))
            return DataNotReady();

        return Ok(await reports.CountByRegionAsync(filter, cancellationToken));
    }

    [HttpGet("by-city")]
    public async Task<ActionResult<IReadOnlyList<CityReport>>> ByCity(
        [FromQuery] SightingFilter filter, CancellationToken cancellationToken)
    {
        if (!await queries.HasAnyDataAsync(cancellationToken))
            return DataNotReady();

        return Ok(await reports.CountByCityAsync(filter, cancellationToken));
    }

    [HttpGet("trends")]
    public async Task<ActionResult<IReadOnlyList<TrendPoint>>> Trends(
        [FromQuery] TrendQuery query, CancellationToken cancellationToken)
    {
        if (!await queries.HasAnyDataAsync(cancellationToken))
            return DataNotReady();

        return Ok(await reports.TrendAsync(query, cancellationToken));
    }

    [HttpGet("seasonal")]
    public async Task<ActionResult<IReadOnlyList<MonthCount>>> Seasonal(
        [FromQuery] SightingFilter filter, CancellationToken cancellationToken)
    {
        if (!await queries.HasAnyDataAsync(cancellationToken))
            return DataNotReady();

        return Ok(await reports.SeasonalAsync(filter, cancellationToken));
    }

    private ObjectResult DataNotReady() =>
        Problem(
            title: "Sighting data is still loading",
            detail: "The first download from the Elanco API hasn't finished yet. Try again in a minute.",
            statusCode: StatusCodes.Status503ServiceUnavailable);
}
