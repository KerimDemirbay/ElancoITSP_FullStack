using Microsoft.AspNetCore.Mvc;
using TickSightings.Data;

namespace TickSightings.Controllers;

[ApiController]
[Route("api/reference")]
public class ReferenceController : ControllerBase
{
    [HttpGet("cities")]
    public ActionResult<IReadOnlyList<CityInfo>> Cities() => Ok(UkCities.All);

    [HttpGet("regions")]
    public ActionResult<IEnumerable<string>> Regions() => Ok(UkCities.Regions);

    [HttpGet("species")]
    public ActionResult<IReadOnlyList<SpeciesInfo>> Species() => Ok(TickSpecies.All);
}