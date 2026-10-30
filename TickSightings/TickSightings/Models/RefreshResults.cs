namespace TickSightings.Models;

public record RefreshSuccess(
    DateTimeOffset RefreshedAt,
    DataQualityReport Report);

public record RefreshFailure(
    DateTimeOffset FailedAt,
    string Message);