namespace CRM.Medical.API.Middlewares;

public static class MiddlewareRegistrationExtensions
{
    public static IServiceCollection AddCrmMiddlewares(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CorrelationIdMiddlewareOptions>(
            configuration.GetSection(CorrelationIdMiddlewareOptions.SectionName));
        services.Configure<SecurityHeadersMiddlewareOptions>(
            configuration.GetSection(SecurityHeadersMiddlewareOptions.SectionName));

        return services;
    }

    public static IApplicationBuilder UseCrmMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<SecurityHeadersMiddleware>();
        return app;
    }
}
