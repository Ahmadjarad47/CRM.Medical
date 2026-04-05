using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CRM.Medical.Infrastructure.Seeding;

/// <summary>
/// Ensures every built-in permission constant exists in the <c>permissions</c> catalog so assignment validators and admin UI stay in sync.
/// </summary>
public sealed class PermissionCatalogSeedHostedService(
    IServiceProvider services,
    ILogger<PermissionCatalogSeedHostedService> logger)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MedicalDbContext>();
        var clock = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

        foreach (var name in UserPermissions.All)
        {
            var exists = await db.Permissions.AnyAsync(p => p.Name == name, cancellationToken);
            if (exists)
                continue;

            db.Permissions.Add(new Permission
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = null,
                CreatedAt = clock.UtcNow
            });

            logger.LogInformation("Seeded permission catalog entry: {Permission}", name);
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
