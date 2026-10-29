using Microsoft.AspNetCore.Mvc;
using TickSightings.Models;
using TickSightings.Services;

namespace TickSightings.Controllers;

[ApiController]
[Route("api/debug")]
public class DebugController(ElancoApiClient elanco) : ControllerBase
{
    [HttpGet("elanco")]
    public async Task<IActionResult> GetCleanedElancoData(CancellationToken cancellationToken)
    {
        var raw = await elanco.GetAllSightingsAsync(cancellationToken);
        var result = SightingCleaner.Clean(raw, DateTime.Now);
        return Ok(new { result.Report, firstFive = result.Sightings.Take(5) });
    }

    [HttpGet("cleaner-demo")]
    public IActionResult CleanerDemo()
    {
        ElancoSighting[] dirty =
        [
            new("a1", "2023-06-01T10:00:00", "London",   "Marsh tick", "Ixodes apronophorus"),
            new("a1", "2023-06-01T10:00:00", "London",   "Marsh tick", "Ixodes apronophorus"),
            new("a2", "2023-06-01T10:00:00", " london ", "marsh tick", null),
            new("a3", null,                  "Leeds",    "Marsh tick", "Ixodes apronophorus"),
            new("a4", "2023-13-45T00:00:00", "Leeds",    "Marsh tick", "Ixodes apronophorus"),
            new("a5", "2099-01-01T00:00:00", "Leeds",    "Marsh tick", "Ixodes apronophorus"),
            new("a6", "2023-06-02T09:00:00", "Atlantis", "Marsh tick", "Ixodes apronophorus"),
            new("a7", "2023-06-02T09:00:00", "Glasgow",  null,         "Ixodes canisuga"),
            new(null, "2023-06-02T09:00:00", "Leeds",    "Marsh tick", null),
        ];

        return Ok(SightingCleaner.Clean(dirty, DateTime.Now));
    }
}