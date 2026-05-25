using MediatR;

namespace CRM.Medical.Application.Features.Dashboard.Queries.GetDashboard;

public sealed record GetDashboardQuery() : IRequest<DashboardResponse>;
