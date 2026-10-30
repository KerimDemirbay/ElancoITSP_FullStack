using System.ComponentModel.DataAnnotations;
using TickSightings.Data;

namespace TickSightings.Models;

public class SightingFilter : IValidatableObject
{
    public DateOnly? From { get; init; }
    public DateOnly? To { get; init; }
    public string? City { get; init; }
    public string? Region { get; init; }
    public string? Species { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (From > To)
            yield return new ValidationResult("'from' must be on or before 'to'.", [nameof(From), nameof(To)]);

        if (City is not null && UkCities.Find(City) is null)
            yield return new ValidationResult(
                $"Unknown city '{City}'. Valid cities: {string.Join(", ", UkCities.All.Select(c => c.Name))}.",
                [nameof(City)]);

        if (Region is not null && UkCities.FindRegion(Region) is null)
            yield return new ValidationResult(
                $"Unknown region '{Region}'. Valid regions: {string.Join(", ", UkCities.Regions)}.",
                [nameof(Region)]);

        if (Species is not null && TickSpecies.Find(Species) is null)
            yield return new ValidationResult(
                $"Unknown species '{Species}'. Valid species: {string.Join(", ", TickSpecies.All.Select(s => s.Name))}.",
                [nameof(Species)]);
    }
}