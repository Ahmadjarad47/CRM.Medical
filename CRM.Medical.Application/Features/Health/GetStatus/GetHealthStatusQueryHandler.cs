using CRM.Medical.Application.Health;
using MediatR;
using Microsoft.Extensions.Hosting;

namespace CRM.Medical.Application.Features.Health.GetStatus;

public sealed class GetHealthStatusQueryHandler(
    IDatabaseHealthSnapshotProvider databaseHealth,
    IHostEnvironment hostEnvironment) : IRequestHandler<GetHealthStatusQuery, HealthStatusViewModel>
{
    public Task<HealthStatusViewModel> Handle(GetHealthStatusQuery request, CancellationToken cancellationToken)
    {
        var db = databaseHealth.GetSnapshot();
        var ok = db.Verified && db.Success;

        var statusPlain = !db.Verified
            ? "Database: startup check not finished yet."
            : ok
                ? "Database: connected — PostgreSQL accepted a query."
                : $"Database: not reachable. {db.ErrorMessage ?? "Unknown error."}";

        var statusClass = !db.Verified ? "pending" : ok ? "ok" : "bad";

        var model = new HealthStatusViewModel(hostEnvironment.EnvironmentName, statusPlain, statusClass);
        return Task.FromResult(model);
    }
}
