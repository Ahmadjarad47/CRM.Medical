using CRM.Medical.Application.Health;
using MediatR;
using Microsoft.Extensions.Hosting;

namespace CRM.Medical.Application.Features.Health.GetStatus;

public sealed class GetHealthStatusQueryHandler(
    IDatabaseHealthSnapshotProvider snapshotProvider,
    IHostEnvironment environment)
    : IRequestHandler<GetHealthStatusQuery, HealthStatusViewModel>
{
    public Task<HealthStatusViewModel> Handle(GetHealthStatusQuery request, CancellationToken cancellationToken)
    {
        var snapshot = snapshotProvider.GetLatest();
        var dbStatus = new DatabaseStatus(snapshot.IsHealthy, snapshot.ErrorMessage, snapshot.CheckedAt);
        var overallStatus = snapshot.IsHealthy ? "Healthy" : "Degraded";
        var httpStatus = snapshot.IsHealthy ? 200 : 503;

        return Task.FromResult(new HealthStatusViewModel(
            overallStatus,
            environment.EnvironmentName,
            DateTime.UtcNow,
            dbStatus,
            httpStatus));
    }
}
