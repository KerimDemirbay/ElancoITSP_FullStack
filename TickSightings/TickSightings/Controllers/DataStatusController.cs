using Microsoft.AspNetCore.Mvc;
using TickSightings.Models;
using TickSightings.Services;

namespace TickSightings.Controllers;

[ApiController]
[Route("api/data-status")]
public class DataStatusController(SightingStore store) : ControllerBase
{
    [HttpGet]
    public ActionResult<DataStatusResponse> Get()
    {
        var current = store.Current;

        return new DataStatusResponse(
            IsReady: current is not null,
            LastRefreshedAt: current?.RefreshedAt,
            SightingCount: current?.Sightings.Count ?? 0,
            Quality: current?.Report,
            LastFailure: store.LastFailure);
    }
}