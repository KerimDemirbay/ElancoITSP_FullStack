using Microsoft.AspNetCore.Mvc;
using TickSightings.Models;
using TickSightings.Services;

namespace TickSightings.Controllers;

[ApiController]
[Route("api/data-status")]
public class DataStatusController(SightingQueryService queries, RefreshStatus status) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<DataStatusResponse>> Get(CancellationToken cancellationToken)
    {
        var sightingCount = await queries.CountAllAsync(cancellationToken);
        var lastSuccess = status.LastSuccess;

        return new DataStatusResponse(
            IsReady: sightingCount > 0,
            LastRefreshedAt: lastSuccess?.RefreshedAt,
            SightingCount: sightingCount,
            Quality: lastSuccess?.Report,
            LastFailure: status.LastFailure);
    }
}