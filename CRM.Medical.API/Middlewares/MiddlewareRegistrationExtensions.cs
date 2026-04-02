namespace CRM.Medical.API.Middlewares;

public static class MiddlewareRegistrationExtensions
{
    public static IServiceCollection AddCrmMiddlewares(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<CorrelationIdMiddlewareOptions>(
            configuration.GetSection("Middlewares:CorrelationId"));

        services.Configure<SecurityHeadersMiddlewareOptions>(
            configuration.GetSection("Middlewares:SecurityHeaders"));

        return services;
    }

    public static IApplicationBuilder UseCrmMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<SecurityHeadersMiddleware>();
        return app;
    }
}
