using TickSightings.Database;
using TickSightings.Models;
using TickSightings.Data;

namespace TickSightings.Services;


public class SightingSubmissionService(TickSightingsDbContext db)
{
    public async Task<SightingSubmissionResponse> SubmitAsync(SightingSubmission submission, CancellationToken cancellationToken)
    {
        var city = UkCities.Find(submission.City)!;
        var species = TickSpecies.Find(submission.Species)!;

        var entity = new SightingEntity
        {
            Id = Guid.NewGuid().ToString(),
            Date = submission.Date!.Value,
            City = city.Name,
            Region = city.Region,
            Species = species.Name,
            LatinName = species.LatinName,
            Source = "Community",
            Notes = string.IsNullOrWhiteSpace(submission.Notes) ? null : submission.Notes.Trim(),
        };

        db.Sightings.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return new SightingSubmissionResponse(entity.ToSighting(), BuildWarnings(submission));
    }

    private static List<string> BuildWarnings(SightingSubmission submission)
    {
        var warnings = new List<string>();

        if (submission.Date < DateTime.Now.AddYears(-1))
            warnings.Add("The saved sighting is more than a year old. Recent reports would be appreciated!");

        if (string.IsNullOrWhiteSpace(submission.Notes))
            warnings.Add("No notes were added. Details about tick sightings would be appreciated!");

        return warnings;
    }

}
