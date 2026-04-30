using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        var db = scope.ServiceProvider.GetRequiredService<MedicalDbContext>();

        // Seed the primary admin
        await SeedUserAsync(
            userManager, roleManager, db, dateTimeProvider,
            seedOptions.Email, seedOptions.Password, seedOptions.DisplayName,
            UserRoles.Admin,
            allPermissions: true,
            cancellationToken);

        // Seed any additional configured users
        foreach (var entry in seedOptions.AdditionalUsers)
        {
            await SeedUserAsync(
                userManager, roleManager, db, dateTimeProvider,
                entry.Email, entry.Password, entry.DisplayName,
                entry.Role,
                entry.AllPermissions,
                cancellationToken);
        }
    }

    private async Task SeedUserAsync(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        MedicalDbContext db,
        IDateTimeProvider dateTimeProvider,
        string email, string password, string displayName,
        string role,
        bool allPermissions,
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

        if (allPermissions)
        {
            await EnsurePermissionCatalogAsync(db, dateTimeProvider, cancellationToken);

            var identityRole = await roleManager.FindByNameAsync(role);
            if (identityRole is null)
            {
                logger.LogWarning("Role '{Role}' not found — skipped assigning catalog permissions.", role);
            }
            else
            {
                var names = UserPermissions.All.ToList();
                var permissionIds = await db.Permissions
                    .Where(p => names.Contains(p.Name))
                    .Select(p => p.Id)
                    .ToListAsync(cancellationToken);

                foreach (var permissionId in permissionIds)
                {
                    var exists = await db.RolePermissions.AnyAsync(
                        rp => rp.RoleId == identityRole.Id && rp.PermissionId == permissionId,
                        cancellationToken);
                    if (exists)
                        continue;

                    db.RolePermissions.Add(new RolePermission
                    {
                        RoleId = identityRole.Id,
                        PermissionId = permissionId
                    });
                }

                await db.SaveChangesAsync(cancellationToken);
            }
        }

        logger.LogInformation(
            "Seeded {Role} user '{Email}' with {Permissions}.",
            role, email,
            allPermissions ? $"{UserPermissions.All.Count} permissions" : "role-only");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task EnsurePermissionCatalogAsync(
        MedicalDbContext db,
        IDateTimeProvider dateTimeProvider,
        CancellationToken cancellationToken)
    {
        var existing = await db.Permissions
            .Select(p => p.Name)
            .ToListAsync(cancellationToken);
        var set = existing.ToHashSet(StringComparer.Ordinal);

        foreach (var name in UserPermissions.All)
        {
            if (set.Contains(name))
                continue;

            db.Permissions.Add(new Permission
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = null,
                CreatedAt = dateTimeProvider.UtcNow
            });
            set.Add(name);
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
