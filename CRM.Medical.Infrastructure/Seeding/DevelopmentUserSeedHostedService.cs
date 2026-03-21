using CRM.Medical.Domain.Entities;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CRM.Medical.Infrastructure.Seeding;

public sealed class DevelopmentUserSeedHostedService(
    IServiceScopeFactory scopeFactory,
    IHostEnvironment hostEnvironment,
    IOptions<DevelopmentSeedOptions> options,
    ILogger<DevelopmentUserSeedHostedService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!hostEnvironment.IsDevelopment() || !options.Value.Enabled)
            return;

        var seed = options.Value;
        if (string.IsNullOrWhiteSpace(seed.Email) || string.IsNullOrWhiteSpace(seed.Password))
        {
            logger.LogWarning(
                "Development seed is enabled but Email or Password is missing. Skipping user seed.");
            return;
        }

        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<MedicalDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await db.Database.MigrateAsync(cancellationToken);

        if (await userManager.FindByEmailAsync(seed.Email) is not null)
        {
            logger.LogInformation("Development seed user already exists ({Email}). Skipping.", seed.Email);
            return;
        }

        var user = new User
        {
            UserName = seed.Email,
            Email = seed.Email,
            EmailConfirmed = true,
            DisplayName = string.IsNullOrWhiteSpace(seed.DisplayName) ? "Test user" : seed.DisplayName,
        };

        var result = await userManager.CreateAsync(user, seed.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
            logger.LogError("Failed to create development seed user: {Errors}", errors);
            return;
        }

        if (await roleManager.RoleExistsAsync(DefaultIdentityRoles.User))
        {
            var roleResult = await userManager.AddToRoleAsync(user, DefaultIdentityRoles.User);
            if (!roleResult.Succeeded)
            {
                var errors = string.Join("; ", roleResult.Errors.Select(e => $"{e.Code}: {e.Description}"));
                logger.LogWarning("Seed user created but assigning role failed: {Errors}", errors);
            }
        }

        logger.LogInformation(
            "Development seed user created. Email: {Email}. Use this account only in Development.",
            seed.Email);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
