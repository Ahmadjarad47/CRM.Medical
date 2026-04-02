using System.Security.Claims;
using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CRM.Medical.Infrastructure.Seeding;

public sealed class DevelopmentUserSeedHostedService(
    IServiceProvider services,
    ILogger<DevelopmentUserSeedHostedService> logger)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = services.CreateScope();
        var seedOptions = scope.ServiceProvider
            .GetRequiredService<IOptions<DevelopmentSeedOptions>>().Value;

        if (!seedOptions.Enabled)
            return;

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

        var existing = await userManager.FindByEmailAsync(seedOptions.Email);
        if (existing is not null)
            return;

        var user = new User
        {
            UserName = seedOptions.Email,
            Email = seedOptions.Email,
            FullName = seedOptions.DisplayName,
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = dateTimeProvider.UtcNow
        };

        var result = await userManager.CreateAsync(user, seedOptions.Password);
        if (!result.Succeeded)
        {
            logger.LogError(
                "Failed to seed development user '{Email}': {Errors}",
                seedOptions.Email,
                string.Join(", ", result.Errors.Select(e => e.Description)));
            return;
        }

        // Assign Admin role for classification
        await userManager.AddToRoleAsync(user, UserRoles.Admin);

        // Assign all permissions as user-level claims
        var permissionClaims = UserPermissions.All
            .Select(p => new Claim(UserPermissions.ClaimType, p))
            .ToList();

        var claimsResult = await userManager.AddClaimsAsync(user, permissionClaims);
        if (!claimsResult.Succeeded)
        {
            logger.LogWarning(
                "Seeded user '{Email}' but failed to assign permission claims: {Errors}",
                seedOptions.Email,
                string.Join(", ", claimsResult.Errors.Select(e => e.Description)));
            return;
        }

        logger.LogInformation(
            "Seeded development admin user '{Email}' with {Count} permissions.",
            seedOptions.Email,
            UserPermissions.All.Count);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
