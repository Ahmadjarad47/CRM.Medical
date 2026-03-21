using CRM.Medical.API.Endpoints;
using CRM.Medical.API.RouteGroups.Auth;
using CRM.Medical.API.RouteGroups.Health;
using CRM.Medical.API.RouteGroups.Users;

namespace CRM.Medical.API.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static WebApplication MapCrmEndpoints(this WebApplication app)
    {
        app.MapRootEndpoints();
        app.MapAuthRouteGroup();
        app.MapUsersRouteGroup();
        app.MapHealthRouteGroup();
        return app;
    }
}
