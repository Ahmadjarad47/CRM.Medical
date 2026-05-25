namespace CRM.Medical.Application.Features.Dashboard.Queries.GetDashboard;

public interface IDashboardReadService
{
    Task<DashboardResponse> GetDashboardAsync(string role, string userId, CancellationToken cancellationToken);
}
