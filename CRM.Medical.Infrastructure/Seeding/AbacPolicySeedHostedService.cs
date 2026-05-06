using CRM.Medical.Application.Features.Users.Constants;
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
                Name = "Admin full access",
                Resource = "*",
                Action = "*",
                SubjectType = SubjectType.Role,
                SubjectId = UserRoles.Admin,
                Effect = PolicyEffect.Allow,
                Priority = 1_000_000,
                Condition = null,
                IsEnabled = true,
                Description = "Admin role has full access to all resources and actions."
            },
            cancellationToken);

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
                Name = "Auditors can view permissions",
                Resource = "Permission",
                Action = "View",
                Effect = PolicyEffect.Allow,
                SubjectType = SubjectType.Role,
                SubjectId = "Auditor",
                Description = "Auditors can view permission catalog.",
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
                 && x.Effect == candidate.Effect
                 && (
                     (x.Condition == null && candidate.Condition == null)
                     || x.Condition == candidate.Condition
                 ),
            cancellationToken);

        if (exists)
            return;

        db.AccessPolicies.Add(candidate);
        await db.SaveChangesAsync(cancellationToken);
    }
}
