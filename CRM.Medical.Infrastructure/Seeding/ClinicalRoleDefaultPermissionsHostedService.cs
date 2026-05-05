using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Domain.Enums;
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
        var dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

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

            await EnsureRolePolicyAsync(db, dateTimeProvider, roleName, cancellationToken);
        }
    }

    private async Task EnsureRolePolicyAsync(
        MedicalDbContext db,
        IDateTimeProvider dateTimeProvider,
        string roleName,
        CancellationToken cancellationToken)
    {
        var exists = await db.AccessPolicies.AnyAsync(
            p => p.Resource == "ExternalPatient"
                 && p.Action == "Manage"
                 && p.SubjectType == SubjectType.Role
                 && p.SubjectId == roleName
                 && p.Effect == PolicyEffect.Allow
                 && p.IsEnabled,
            cancellationToken);
        if (exists)
            return;

        db.AccessPolicies.Add(new AccessPolicy
        {
            Id = Guid.NewGuid(),
            Name = $"{roleName} can manage external patients",
            Resource = "ExternalPatient",
            Action = "Manage",
            SubjectType = SubjectType.Role,
            SubjectId = roleName,
            Effect = PolicyEffect.Allow,
            Priority = 200,
            IsEnabled = true,
            Description = "Manage external patient records (list, create, link).",
            CreatedAt = dateTimeProvider.UtcNow,
            Condition = $$"""{"in":["{{roleName}}","user.roles"]}"""
        });
        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Added ABAC role policy for {Role}.",
            roleName);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
