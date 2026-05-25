using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Users.Constants;
using MediatR;

namespace CRM.Medical.Application.Features.Dashboard.Queries.GetDashboard;

public sealed class GetDashboardQueryHandler(
    ICurrentUserAccessor currentUser,
    IDashboardReadService dashboardReadService)
    : IRequestHandler<GetDashboardQuery, DashboardResponse>
{
    public Task<DashboardResponse> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetRequiredUserId();
        var role = ResolveDashboardRole(currentUser);
        return dashboardReadService.GetDashboardAsync(role, userId, cancellationToken);
    }

    private static string ResolveDashboardRole(ICurrentUserAccessor currentUser)
    {
        if (currentUser.IsInRole(UserRoles.Admin))
            return UserRoles.Admin;
        if (currentUser.IsInRole(UserRoles.Doctor))
            return UserRoles.Doctor;
        if (currentUser.IsInRole(UserRoles.LabPartner))
            return UserRoles.LabPartner;
        if (currentUser.IsInRole(UserRoles.Patient))
            return UserRoles.Patient;

        throw new ApplicationUnauthorizedException("Unable to identify the current user's dashboard role.");
    }
}
