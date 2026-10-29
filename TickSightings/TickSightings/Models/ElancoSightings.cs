namespace TickSightings.Models;

public record ElancoSighting(
    string? Id,
    string? Date,
    string? Location,
    string? Species,
    string? LatinName
);