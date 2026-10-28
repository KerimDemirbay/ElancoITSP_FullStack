namespace TickSightings.Models;

public record class ElancoSighting(
    string? Id,
    string? Date,
    string? Location,
    string? Species,
    string? LatinName
);