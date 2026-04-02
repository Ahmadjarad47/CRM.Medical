using CRM.Medical.Application.Health;

namespace CRM.Medical.Infrastructure.Diagnostics;

public sealed class DatabaseHealthSnapshotProvider(DatabaseConnectionReport report)
    : IDatabaseHealthSnapshotProvider
{
    public DatabaseHealthSnapshot GetLatest() =>
        new(report.IsConnected, report.LastError, report.LastChecked);
}
