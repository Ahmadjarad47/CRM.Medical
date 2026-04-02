using System.Text;
using CRM.Medical.Application.Auth;
using CRM.Medical.Application.Features.Users.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace CRM.Medical.API.Extensions;

public static class JwtAuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>()
            ?? throw new InvalidOperationException("JWT settings are not configured.");

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async ctx =>
                    {
                        ctx.HandleResponse();
                        ctx.Response.StatusCode = 401;
                        ctx.Response.ContentType = "application/problem+json";
                        await ctx.Response.WriteAsJsonAsync(new
                        {
                            type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                            title = "Unauthorized",
                            status = 401,
                            detail = "A valid Bearer token is required."
                        });
                    }
                };
            });

        // Policy-based authorization: one policy per permission claim value.
        // Endpoints use [Authorize(Policy = UserPermissions.UsersView)] etc.
        // Roles are NOT used for authorization — only for classification/filtering.
        services.AddAuthorization(options =>
        {
            foreach (var permission in UserPermissions.All)
            {
                options.AddPolicy(permission, policy =>
                    policy.RequireClaim(UserPermissions.ClaimType, permission));
            }
        });

        return services;
    }
}
