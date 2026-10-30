using TickSightings.Models;

namespace TickSightings.Database;

public class SightingEntity
{
    public required string Id { get; set; }
    public DateTime Date { get; set; }
    public required string City { get; set; }
    public required string Region { get; set; }
    public required string Species { get; set; }
    public required string LatinName { get; set; }
    public required string Source { get; set; }

    //Only filled in for community reports 
    public string? Notes { get; set; }

    public static SightingEntity FromSighting(Sighting sighting) => new()
    {
        Id = sighting.Id,
        Date = sighting.Date,
        City = sighting.City,
        Region = sighting.Region,
        Species = sighting.Species,
        LatinName = sighting.LatinName,
        Source = sighting.Source,
    };

    public Sighting ToSighting() => new(Id, Date, City, Region, Species, LatinName, Source, Notes);
}