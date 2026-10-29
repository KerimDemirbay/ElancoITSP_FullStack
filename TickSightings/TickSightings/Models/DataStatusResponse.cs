namespace TickSightings.Models;

public record DataStatusResponse(
    bool IsReady,
    DateTimeOffset? LastRefreshedAt,
    int SightingCount,
    DataQualityReport? Quality,
    RefreshFailure? LastFailure  
);