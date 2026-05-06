using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Infrastructure.Persistence;
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
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

        await SeedUserAsync(
            userManager, roleManager, dateTimeProvider,
            seedOptions.Email, seedOptions.Password, seedOptions.DisplayName,
            UserRoles.Admin,
            cancellationToken);

        foreach (var entry in seedOptions.AdditionalUsers)
        {
            await SeedUserAsync(
                userManager, roleManager, dateTimeProvider,
                entry.Email, entry.Password, entry.DisplayName,
                entry.Role,
                cancellationToken);
        }
    }

    private async Task SeedUserAsync(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        IDateTimeProvider dateTimeProvider,
        string email, string password, string displayName,
        string role,
        CancellationToken cancellationToken)
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

        logger.LogInformation(
            "Seeded {Role} user '{Email}' (effective access is determined by AccessPolicy at runtime).",
            role,
            email);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
