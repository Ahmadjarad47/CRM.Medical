using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Features.Permissions.Services;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CRM.Medical.Infrastructure.Seeding;

/// <summary>
/// Ensures workflow permissions required by Doctor / LabPartner roles exist in the catalog and are assigned to those roles.
/// Runs once per application start (idempotent).
/// </summary>
public sealed class ClinicalRoleDefaultPermissionsHostedService(
    IServiceProvider services,
    ILogger<ClinicalRoleDefaultPermissionsHostedService> logger)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MedicalDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var rolePermissions = scope.ServiceProvider.GetRequiredService<IRolePermissionService>();
        var dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

        await EnsurePermissionInCatalogAsync(db, dateTimeProvider, cancellationToken);

        var permissionId = await db.Permissions.AsNoTracking()
            .Where(p => p.Name == UserPermissions.ExternalPatientsManage)
            .Select(p => p.Id)
            .SingleAsync(cancellationToken);

        foreach (var roleName in new[] { UserRoles.Doctor, UserRoles.LabPartner })
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role is null)
            {
                logger.LogWarning(
                    "Role '{Role}' was not found — skipped assigning {Permission}.",
                    roleName,
                    UserPermissions.ExternalPatientsManage);
                continue;
            }

            await rolePermissions.AssignPermissionToRoleAsync(role.Id, permissionId, cancellationToken);
        }
    }

    private async Task EnsurePermissionInCatalogAsync(
        MedicalDbContext db,
        IDateTimeProvider dateTimeProvider,
        CancellationToken cancellationToken)
    {
        var exists = await db.Permissions.AnyAsync(
            p => p.Name == UserPermissions.ExternalPatientsManage,
            cancellationToken);
        if (exists)
            return;

        db.Permissions.Add(new Permission
        {
            Id = Guid.NewGuid(),
            Name = UserPermissions.ExternalPatientsManage,
            Description = "Manage external patient records (list, create, link).",
            CreatedAt = dateTimeProvider.UtcNow
        });
        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Added permission catalog entry {Permission}.",
            UserPermissions.ExternalPatientsManage);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
