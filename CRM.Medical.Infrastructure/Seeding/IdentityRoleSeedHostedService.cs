using CRM.Medical.Application.Features.Users.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CRM.Medical.Infrastructure.Seeding;

public sealed class IdentityRoleSeedHostedService(
    IServiceProvider services,
    ILogger<IdentityRoleSeedHostedService> logger)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var roleName in UserRoles.All)
        {
            if (await roleManager.RoleExistsAsync(roleName))
                continue;

            var result = await roleManager.CreateAsync(new IdentityRole(roleName));

            if (result.Succeeded)
                logger.LogInformation("Created identity role: {Role}", roleName);
            else
                logger.LogError(
                    "Failed to create role '{Role}': {Errors}",
                    roleName,
                    string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
