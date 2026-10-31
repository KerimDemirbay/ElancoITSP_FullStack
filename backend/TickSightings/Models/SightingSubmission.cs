using System.ComponentModel.DataAnnotations;
using TickSightings.Data;

namespace TickSightings.Models;

public record SightingSubmissionResponse(
    Sighting Sighting,
    IReadOnlyList<string> Warnings
);
public class SightingSubmission :IValidatableObject
{
    [Required]
    public DateTime? Date { get; init; }

    [Required]
    public string? City { get; init; }

    [Required]
    public string? Species { get; init; }

    [StringLength(500)]
    public string? Notes { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Date > DateTime.Now)
            yield return new ValidationResult("The sighting date can't be in the future", [nameof(Date)]);

        if (Date < new DateTime(2000, 1, 1))
            yield return new ValidationResult("The sighting date must be in the year 2000 or later", [nameof(Date)]);

        if (TickSpecies.Find(Species) is null)
            yield return new ValidationResult(
                $"Unknown Species '{Species}'. If you wish to add a tick of new species please contact us. Valid species: {string.Join(", ", TickSpecies.All.Select(s => s.Name))}.",
                [nameof(Species)]);

        if (TickSpecies.Find(City) is null)
            yield return new ValidationResult(
                $"Unknown city '{City}'. Valid cities: {string.Join(", ", UkCities.All.Select(c => c.Name))}.",
                [nameof(City)]);
    }
}
