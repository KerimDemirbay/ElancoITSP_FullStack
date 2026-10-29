using TickSightings.Models;
namespace TickSightings.Services;

public class SightingStore
{
    private volatile DataSnapshot? _current;
    private volatile RefreshFailure? _lastFailure;

    public DataSnapshot? Current => _current;
    public RefreshFailure? LastFailure => _lastFailure;

    public void ReplaceData(DataSnapshot snapshot)
    {
        _current = Current;
        _lastFailure = null;
    }

    public void RecordFailure(RefreshFailure failure)
    {
        _lastFailure = failure;
    }
}
