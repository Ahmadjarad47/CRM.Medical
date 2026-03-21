using CRM.Medical.API.Endpoints.Users;

namespace CRM.Medical.API.RouteGroups.Users;

public static class UsersRouteGroup
{
    public static IEndpointRouteBuilder MapUsersRouteGroup(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users")
            .WithGroupName("v1")
            .RequireAuthorization();
        group.MapUserQueryEndpoints();
        group.MapUserProfileEndpoints();
        group.MapUserRoleEndpoints();
        group.MapUserPermissionEndpoints();
        group.MapUserLockoutEndpoints();
        group.MapUserEmailVerificationEndpoints();
        return app;
    }
}
