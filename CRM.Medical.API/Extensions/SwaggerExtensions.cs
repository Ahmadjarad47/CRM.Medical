using Microsoft.AspNetCore.Authentication.JwtBearer;
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

            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                    Description =
                        "JWT from POST /api/auth/login (use the accessToken value). Paste the token only — Swagger sends Authorization: Bearer.",
                });
        });

        return services;
    }
}
