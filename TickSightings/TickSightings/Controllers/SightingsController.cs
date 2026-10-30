using Microsoft.AspNetCore.Mvc;
using TickSightings.Models;
using TickSightings.Services;

namespace TickSightings.Controllers;

[ApiController]
[Route("api/sightings")]
public class SightingsController(SightingQueryService queries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<SightingSearchResponse>> Search(
    [FromQuery] SightingSearchQuery query, CancellationToken cancellationToken)
    {
        if (!await queries.HasAnyDataAsync(cancellationToken))
            return DataNotReady();

        var (items, totalCount) = await queries.SearchAsync(query, cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

        return new SightingSearchResponse(items, query.Page, query.PageSize, totalCount, totalPages);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Sighting>> GetById(string id, CancellationToken cancellationToken)
    {
        if (!await queries.HasAnyDataAsync(cancellationToken))
            return DataNotReady();

        var sighting = await queries.FindByIdAsync(id, cancellationToken);

        if (sighting is null)
            return NotFound();

        return sighting;
    }

    private ObjectResult DataNotReady() =>
        Problem(
            title: "Sighting data is still loading",
            detail: "The first download from the Elanco API hasn't finished yet. Try again in a minute.",
            statusCode: StatusCodes.Status503ServiceUnavailable);
}