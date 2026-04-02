namespace CRM.Medical.Application.Health;

public sealed record DatabaseHealthSnapshot(
    bool IsHealthy,
    string? ErrorMessage,
    DateTime CheckedAt);

public interface IDatabaseHealthSnapshotProvider
{
    DatabaseHealthSnapshot GetLatest();
}
