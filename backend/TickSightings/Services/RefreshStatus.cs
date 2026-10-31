using TickSightings.Models;

namespace TickSightings.Services;

public class RefreshStatus
{
    private volatile RefreshSuccess? _lastSuccess;
    private volatile RefreshFailure? _lastFailure;

    public RefreshSuccess? LastSuccess => _lastSuccess;
    public RefreshFailure? LastFailure => _lastFailure;

    public void RecordSuccess(RefreshSuccess success)
    {
        _lastSuccess = success;
        _lastFailure = null;
    }

    public void RecordFailure(RefreshFailure failure)
    {
        _lastFailure = failure;
    }
}