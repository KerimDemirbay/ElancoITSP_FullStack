namespace TickSightings.Models;

public record Sighting(
    string Id,
    DateTime Date,
    string City,
    string Region,
    string Species,
    string LatinName,
    string Source
);