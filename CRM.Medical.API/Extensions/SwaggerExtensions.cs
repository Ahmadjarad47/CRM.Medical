using Microsoft.OpenApi;

namespace CRM.Medical.API.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddCrmSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "CRM Medical API",
                Version = "v1",
                Description =
                    "Production-grade CRM Medical REST API — Clean Architecture · CQRS · JWT · Redis. " +
                    "JSON responses use a common envelope: `{ \"success\": true|false, \"message\": \"ok|bad\", \"data\": ... }`. " +
                    "Endpoints that previously returned 204 No Content now return **200** with `data: null` and `message: \"ok\"`."
            });

            // JWT Bearer security definition
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "JWT Bearer token. Enter: **Bearer {token}**",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            // Apply security globally using the new Swashbuckle 10.x factory API
            options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("Bearer", doc),
                    new List<string>()
                }
            });

            options.EnableAnnotations();
        });

        return services;
    }
}
