using CRM.Medical.Domain.Entities;
using CRM.Medical.Domain.Enums;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CRM.Medical.Infrastructure.Seeding;

/// <summary>
/// Seeds baseline ABAC policies (idempotent). These are examples and can be managed from admin APIs later.
/// </summary>
public sealed class AbacPolicySeedHostedService(
    IServiceProvider services,
    ILogger<AbacPolicySeedHostedService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MedicalDbContext>();

        await EnsurePolicyAsync(
            db,
            new AccessPolicy
            {
                Id = Guid.NewGuid(),
                Name = "Doctors can view own test results",
                Resource = "TestResult",
                Action = "Read",
                Effect = PolicyEffect.Allow,
                SubjectType = SubjectType.Role,
                SubjectId = "Doctor",
                Description = "Doctors can view test results they created.",
                Condition = """{"all":[{"in":["Doctor","user.roles"]},{"eq":["user.id","resource.createdByUserId"]}]}""",
                Priority = 200,
                IsEnabled = true
            },
            cancellationToken);

        await EnsurePolicyAsync(
            db,
            new AccessPolicy
            {
                Id = Guid.NewGuid(),
                Name = "Admins can create permissions",
                Resource = "Permission",
                Action = "Create",
                Effect = PolicyEffect.Allow,
                SubjectType = SubjectType.Role,
                SubjectId = "Admin",
                Description = "Admins can create permission policies.",
                Condition = """{"in":["Admin","user.roles"]}""",
                Priority = 260,
                IsEnabled = true
            },
            cancellationToken);

        await EnsurePolicyAsync(
            db,
            new AccessPolicy
            {
                Id = Guid.NewGuid(),
                Name = "Admins can update permissions",
                Resource = "Permission",
                Action = "Update",
                Effect = PolicyEffect.Allow,
                SubjectType = SubjectType.Role,
                SubjectId = "Admin",
                Description = "Admins can update permission policies.",
                Condition = """{"in":["Admin","user.roles"]}""",
                Priority = 260,
                IsEnabled = true
            },
            cancellationToken);

        await EnsurePolicyAsync(
            db,
            new AccessPolicy
            {
                Id = Guid.NewGuid(),
                Name = "Admins can delete permissions",
                Resource = "Permission",
                Action = "Delete",
                Effect = PolicyEffect.Allow,
                SubjectType = SubjectType.Role,
                SubjectId = "Admin",
                Description = "Admins can delete permission policies.",
                Condition = """{"in":["Admin","user.roles"]}""",
                Priority = 260,
                IsEnabled = true
            },
            cancellationToken);

        await EnsurePolicyAsync(
            db,
            new AccessPolicy
            {
                Id = Guid.NewGuid(),
                Name = "Admins can manage roles",
                Resource = "Role",
                Action = "Manage",
                Effect = PolicyEffect.Allow,
                SubjectType = SubjectType.Role,
                SubjectId = "Admin",
                Description = "Admins can manage roles.",
                Condition = """{"in":["Admin","user.roles"]}""",
                Priority = 300,
                IsEnabled = true
            },
            cancellationToken);

        await EnsurePolicyAsync(
            db,
            new AccessPolicy
            {
                Id = Guid.NewGuid(),
                Name = "Admins and auditors can view permissions",
                Resource = "Permission",
                Action = "View",
                Effect = PolicyEffect.Allow,
                SubjectType = SubjectType.Role,
                SubjectId = "Admin",
                Description = "Admins and Auditors can view permission catalog.",
                Condition = """{"in":["Admin","user.roles"]}""",
                Priority = 250,
                IsEnabled = true
            },
            cancellationToken);

        await EnsurePolicyAsync(
            db,
            new AccessPolicy
            {
                Id = Guid.NewGuid(),
                Name = "Auditors can view permissions",
                Resource = "Permission",
                Action = "View",
                Effect = PolicyEffect.Allow,
                SubjectType = SubjectType.Role,
                SubjectId = "Auditor",
                Description = "Admins and Auditors can view permission catalog.",
                Condition = """{"in":["Auditor","user.roles"]}""",
                Priority = 250,
                IsEnabled = true
            },
            cancellationToken);

        logger.LogInformation("ABAC policy seed completed.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static async Task EnsurePolicyAsync(
        MedicalDbContext db,
        AccessPolicy candidate,
        CancellationToken cancellationToken)
    {
        var exists = await db.AccessPolicies.AnyAsync(
            x => x.Resource == candidate.Resource
                 && x.Action == candidate.Action
                 && x.SubjectType == candidate.SubjectType
                 && x.SubjectId == candidate.SubjectId
                 && x.Effect == candidate.Effect,
            cancellationToken);

        if (exists)
            return;

        db.AccessPolicies.Add(candidate);
        await db.SaveChangesAsync(cancellationToken);
    }
}
