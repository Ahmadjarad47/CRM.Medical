using CRM.Medical.API.Endpoints;

namespace CRM.Medical.API.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static WebApplication MapCrmEndpoints(this WebApplication app)
    {
        app.MapRootEndpoints();
        app.MapAuthEndpoints();
        app.MapHealthEndpoints();
        return app;
    }
}
