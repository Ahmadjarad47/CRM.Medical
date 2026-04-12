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

        // Seed the primary admin
        await SeedUserAsync(
            userManager, dateTimeProvider,
            seedOptions.Email, seedOptions.Password, seedOptions.DisplayName,
            UserRoles.Admin, allPermissions: true);

        // Seed any additional configured users
        foreach (var entry in seedOptions.AdditionalUsers)
        {
            await SeedUserAsync(
                userManager, dateTimeProvider,
                entry.Email, entry.Password, entry.DisplayName,
                entry.Role, entry.AllPermissions);
        }
    }

    private async Task SeedUserAsync(
        UserManager<User> userManager,
        IDateTimeProvider dateTimeProvider,
        string email, string password, string displayName,
        string role, bool allPermissions)
    {
        if (string.IsNullOrWhiteSpace(email))
            return;

        var existing = await userManager.FindByEmailAsync(email);
        if (existing is not null)
            return;

        var user = new User
        {
            UserName = email,
            Email = email,
            FullName = displayName,
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = dateTimeProvider.UtcNow
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            logger.LogError(
                "Failed to seed user '{Email}': {Errors}",
                email,
                string.Join(", ", result.Errors.Select(e => e.Description)));
            return;
        }

        await userManager.AddToRoleAsync(user, role);

        if (allPermissions)
        {
            var permissionClaims = UserPermissions.All
                .Select(p => new Claim(UserPermissions.ClaimType, p))
                .ToList();

            var claimsResult = await userManager.AddClaimsAsync(user, permissionClaims);
            if (!claimsResult.Succeeded)
            {
                logger.LogWarning(
                    "Seeded user '{Email}' but failed to assign permission claims: {Errors}",
                    email,
                    string.Join(", ", claimsResult.Errors.Select(e => e.Description)));
                return;
            }
        }

        logger.LogInformation(
            "Seeded {Role} user '{Email}' with {Permissions}.",
            role, email,
            allPermissions ? $"{UserPermissions.All.Count} permissions" : "role-only");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
