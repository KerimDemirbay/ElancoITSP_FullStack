using System.Globalization;
using TickSightings.Data;
using TickSightings.Models;

namespace TickSightings.Services;

public static class SightingCleaner
{
    private static readonly string[] AcceptedDateFormats = ["yyyy-MM-ddTHH:mm:ss", "yyyy-MM-dd"];

    private static (Sighting? Sighting, string? Problem) CleanOne(ElancoSighting raw, DateTime now)
    {
        if (string.IsNullOrWhiteSpace(raw.Id))
            return (null, "Missing id");

        if (!DateTime.TryParseExact(raw.Date?.Trim(), AcceptedDateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            return (null, "Missing or invalid date");

        if (date > now)
            return (null, "Date in the future");

        var city = UkCities.Find(raw.Location);
        if (city is null)
            return (null, "Unknown location");

        var species = TickSpecies.Find(raw.Species) ?? TickSpecies.FindByLatin(raw.LatinName);
        if (species is null)
            return (null, "Unknown species");

        var sighting = new Sighting(
            Id: raw.Id.Trim(),
            Date: date,
            City: city.Name,
            Region: city.Region,
            Species: species.Name,
            LatinName: species.LatinName,
            Source: "Elanco");

        return (sighting, null);
    }

    public static CleaningResult Clean(IEnumerable<ElancoSighting> rawSightings, DateTime now)
    {
        var accepted = new List<Sighting>();
        var rejected = new Dictionary<string, int>();
        var seenIds = new HashSet<string>();
        var seenContent = new HashSet<(DateTime, string, string)>();
        var received = 0;

        foreach (var raw in rawSightings)
        {
            received++;
            var (sighting, problem) = CleanOne(raw, now);
            //if the sighting is null, "problem" cannot also be null
            if (sighting is null)
            {
                Reject(problem!);
                continue;
            }
            //returns false if the ID has already been seen before
            if (!seenIds.Add(sighting.Id))
            {
                Reject("Duplicate id");
                continue;
            }
            //rejects two identical sightings if they have different id's, to stop accidental re-submissions 
            if (!seenContent.Add((sighting.Date, sighting.City, sighting.Species)))
            {
                Reject("Duplicate content");
                continue;
            }

            accepted.Add(sighting);
        }

        return new CleaningResult(accepted, new DataQualityReport(received, accepted.Count, rejected));

        void Reject(string reason) => rejected[reason] = rejected.GetValueOrDefault(reason) + 1;
    }

}