namespace TickSightings.Models;

public record CleaningResult(
    IReadOnlyList<Sighting> Sightings,
    DataQualityReport Report);

public record DataQualityReport(
    int Received,
    int Accepted,
    IReadOnlyDictionary<string, int> Rejected);