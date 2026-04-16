using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using CRM.Medical.Application.Auth;
using CRM.Medical.Application.Common.Caching;
using CRM.Medical.Application.Common.Storage;
using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Configuration.Database;
using CRM.Medical.Application.Configuration.S3;
using CRM.Medical.Application.Features.AppointmentTypes;
using CRM.Medical.Application.Features.Appointments;
using CRM.Medical.Application.Features.Complaints;
using CRM.Medical.Application.Features.MedicalTests;
using CRM.Medical.Application.Features.Banners;
using CRM.Medical.Application.Features.SlideCards;
using CRM.Medical.Application.Features.SubscriptionPackages;
using CRM.Medical.Application.Features.Templates;
using CRM.Medical.Application.Features.TestRequests;
using CRM.Medical.Application.Features.Users.Services;
using CRM.Medical.Application.Health;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Infrastructure.Auth;
using CRM.Medical.Infrastructure.Caching;
using CRM.Medical.Infrastructure.Configuration;
using CRM.Medical.Infrastructure.Diagnostics;
using CRM.Medical.Infrastructure.Email;
using CRM.Medical.Infrastructure.Persistence;
using CRM.Medical.Infrastructure.Seeding;
using CRM.Medical.Infrastructure.Persistence.Repositories;
using CRM.Medical.Infrastructure.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CRM.Medical.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── Database ──────────────────────────────────────────────────────────
        services.AddOptions<DatabaseSettings>();
        services.ConfigureOptions<DatabaseSettingsFromEnvironmentConfigurer>();
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<S3StorageSettings>(configuration.GetSection(S3StorageSettings.SectionName));
        services.AddSingleton<IDatabaseConnectionStringBuilder, NpgsqlDatabaseConnectionStringBuilder>();

        services.AddSingleton<IAmazonS3>(sp =>
        {
            var o = sp.GetRequiredService<IOptions<S3StorageSettings>>().Value;
            var credentials = new BasicAWSCredentials(o.AccessKey, o.SecretKey);

            if (!string.IsNullOrWhiteSpace(o.ServiceUrl))
            {
                var cfg = new AmazonS3Config
                {
                    ServiceURL = o.ServiceUrl,
                    ForcePathStyle = o.ForcePathStyle
                };
                return new AmazonS3Client(credentials, cfg);
            }

            var regionName = string.IsNullOrWhiteSpace(o.Region) ? "us-east-1" : o.Region;
            return new AmazonS3Client(credentials, RegionEndpoint.GetBySystemName(regionName));
        });

        services.AddScoped<IObjectStorageService, S3ObjectStorageService>();
        services.AddScoped<IComplaintRepository, ComplaintRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IAppointmentTypeRepository, AppointmentTypeRepository>();
        services.AddScoped<ISubscriptionPackageRepository, SubscriptionPackageRepository>();
        services.AddScoped<IMedicalTestRepository, MedicalTestRepository>();
        services.AddScoped<ITestRequestRepository, TestRequestRepository>();
        services.AddScoped<ITestResultRepository, TestResultRepository>();
        services.AddScoped<ISlideCardRepository, SlideCardRepository>();
        services.AddScoped<IBannerRepository, BannerRepository>();
        services.AddScoped<ITemplateRepository, TemplateRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();

        services.AddDbContext<MedicalDbContext>((sp, options) =>
        {
            var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
            var csBuilder = sp.GetRequiredService<IDatabaseConnectionStringBuilder>();
            options.UseNpgsql(csBuilder.Build(settings));
        });

        // ── Identity ──────────────────────────────────────────────────────────
        // AddIdentityCore avoids cookie auth — keeps it pure JWT for APIs.
        services
            .AddIdentityCore<User>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false; // set to true when email infra is ready
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
            .AddRoles<IdentityRole>()
            .AddSignInManager()
            .AddEntityFrameworkStores<MedicalDbContext>()
            .AddDefaultTokenProviders();

        // ── Auth services ─────────────────────────────────────────────────────
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IUserCredentialValidator, UserCredentialValidator>();

        // ── Email stubs (replace with real senders in production) ─────────────
        services.AddScoped<IEmailVerificationSender, LoggingEmailVerificationSender>();
        services.AddScoped<IPasswordResetSender, LoggingPasswordResetSender>();
        services.AddScoped<IAccountDeletionSender, LoggingAccountDeletionSender>();

        // ── Redis caching ──────────────────────────────────────────────────────
        var redisConnection = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(redisConnection))
        {
            services.AddStackExchangeRedisCache(opts => opts.Configuration = redisConnection);
        }
        else
        {
            // Fallback to in-memory cache when Redis is not configured (local dev without Docker)
            services.AddDistributedMemoryCache();
        }
        services.AddSingleton<ICacheService, RedisCacheService>();

        // ── DB health diagnostics ─────────────────────────────────────────────
        services.AddSingleton<DatabaseConnectionReport>();
        services.AddSingleton<IDatabaseHealthSnapshotProvider, DatabaseHealthSnapshotProvider>();
        services.AddHostedService<DatabaseConnectivityHostedService>();

        // ── Seeding ───────────────────────────────────────────────────────────
        services.Configure<DevelopmentSeedOptions>(
            configuration.GetSection(DevelopmentSeedOptions.SectionName));
        services.AddHostedService<IdentityRoleSeedHostedService>();
        services.AddHostedService<PermissionCatalogSeedHostedService>();
        services.AddHostedService<DevelopmentUserSeedHostedService>();

        return services;
    }
}
