using CRM.Medical.API.Endpoints.Health;

namespace CRM.Medical.API.RouteGroups.Health;

public static class HealthRouteGroup
{
    public static IEndpointRouteBuilder MapHealthRouteGroup(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(string.Empty)
            .WithTags("Health")
            .WithGroupName("v1");
        group.MapStatusEndpoint();
        return app;
    }
}
