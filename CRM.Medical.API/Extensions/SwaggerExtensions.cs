using Microsoft.OpenApi;

namespace CRM.Medical.API.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddCrmSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "CRM Medical API",
                Version = "v1",
            });
        });

        return services;
    }
}
