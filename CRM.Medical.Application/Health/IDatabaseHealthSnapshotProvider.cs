namespace CRM.Medical.Application.Health;

public interface IDatabaseHealthSnapshotProvider
{
    DatabaseHealthSnapshot GetSnapshot();
}
