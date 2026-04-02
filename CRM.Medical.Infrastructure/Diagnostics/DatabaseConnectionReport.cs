namespace CRM.Medical.Infrastructure.Diagnostics;

public sealed class DatabaseConnectionReport
{
    private volatile bool _isConnected;
    private string? _lastError;
    private DateTime _lastChecked = DateTime.UtcNow;

    public bool IsConnected => _isConnected;
    public string? LastError => _lastError;
    public DateTime LastChecked => _lastChecked;

    public void ReportSuccess()
    {
        _isConnected = true;
        _lastError = null;
        _lastChecked = DateTime.UtcNow;
    }

    public void ReportFailure(string error)
    {
        _isConnected = false;
        _lastError = error;
        _lastChecked = DateTime.UtcNow;
    }
}
