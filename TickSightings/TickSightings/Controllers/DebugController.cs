using Microsoft.AspNetCore.Mvc;
using TickSightings.Services;
namespace TickSightings.Controllers;

[ApiController]
[Route("elancoApi/debug")]
public class DebugController(ElancoApiClient elanco) : ControllerBase
{
    [HttpGet("elanco")]
    public async Task<IActionResult> GetRawElancoData(CancellationToken cancellationToken)
    {
        var sightings = await elanco.GetAllSightingsAsync(cancellationToken);
        return Ok(new
        {
            count = sightings.Count,
            firstFive = sightings.Take(5),
        });
    }
}

