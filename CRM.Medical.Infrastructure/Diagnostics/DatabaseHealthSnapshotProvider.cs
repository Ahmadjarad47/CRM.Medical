using CRM.Medical.Application.Health;

namespace CRM.Medical.Infrastructure.Diagnostics;

public sealed class DatabaseHealthSnapshotProvider(DatabaseConnectionReport report) : IDatabaseHealthSnapshotProvider
{
    public DatabaseHealthSnapshot GetSnapshot() =>
        new(report.Verified, report.Success, report.ErrorMessage);
}
