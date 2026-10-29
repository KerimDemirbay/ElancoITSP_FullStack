namespace TickSightings.Models;

//bundles everything from one successful refresh
//new refresh, new data, old one gets overwritten 
public record DataSnapshot
(
    IReadOnlyList<Sighting> Sightings,
    DataQualityReport Report,
    //in DateTime UTC
    DateTimeOffset RefreshedAt
);

public record RefreshFailure
(
    //in DateTime UTC
    DateTimeOffset FailedAt,
    string Message
);
