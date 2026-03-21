using CRM.Medical.API.Endpoints.Auth;

namespace CRM.Medical.API.RouteGroups.Auth;

public static class AuthRouteGroup
{
    public static IEndpointRouteBuilder MapAuthRouteGroup(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Auth")
            .WithGroupName("v1");
        group.MapLoginEndpoint();
        group.MapCurrentUserEndpoint();
        return app;
    }
}
