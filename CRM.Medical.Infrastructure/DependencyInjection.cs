using CRM.Medical.Application.Auth;
using CRM.Medical.Application.Configuration.Database;
using CRM.Medical.Application.Health;
using CRM.Medical.Application.Features.Users.Services;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Infrastructure.Auth;
using CRM.Medical.Infrastructure.Configuration;
using CRM.Medical.Infrastructure.Diagnostics;
using CRM.Medical.Infrastructure.Persistence;
using CRM.Medical.Infrastructure.Seeding;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace CRM.Medical.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DatabaseSettings>();
        services.ConfigureOptions<DatabaseSettingsFromEnvironmentConfigurer>();

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddSingleton<IDatabaseConnectionStringBuilder, NpgsqlDatabaseConnectionStringBuilder>();

        services.AddDbContext<MedicalDbContext>((serviceProvider, options) =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<DatabaseSettings>>().Value;
            var connectionStringBuilder = serviceProvider.GetRequiredService<IDatabaseConnectionStringBuilder>();
            options.UseNpgsql(connectionStringBuilder.Build(settings));
        });

        // AddIdentity registers cookie auth and overrides default schemes (redirects API to /Account/Login).
        // JWT API: IdentityCore + roles + EF stores — no cookie authentication.
        services
            .AddIdentityCore<User>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
            .AddRoles<IdentityRole>()
            .AddSignInManager()
            .AddEntityFrameworkStores<MedicalDbContext>()
            .AddDefaultTokenProviders();

        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IUserCredentialValidator, UserCredentialValidator>();
        services.AddScoped<IEmailVerificationSender, LoggingEmailVerificationSender>();
        services.AddScoped<IPasswordResetSender, LoggingPasswordResetSender>();
        services.AddSingleton<IDatabaseHealthSnapshotProvider, DatabaseHealthSnapshotProvider>();

        services.Configure<DevelopmentSeedOptions>(configuration.GetSection(DevelopmentSeedOptions.SectionName));
        services.AddHostedService<IdentityRoleSeedHostedService>();
        services.AddHostedService<DevelopmentUserSeedHostedService>();

        services.AddSingleton<DatabaseConnectionReport>();
        services.AddHostedService<DatabaseConnectivityHostedService>();

        return services;
    }
}
