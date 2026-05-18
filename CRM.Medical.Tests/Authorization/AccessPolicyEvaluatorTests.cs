using System.Text.Json;
using CRM.Medical.Application.Authorization;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Infrastructure.Authorization;

namespace CRM.Medical.Tests.Authorization;

public sealed class AccessPolicyEvaluatorTests
{
    [Fact]
    public async Task Evaluator_Should_Apply_Allow_And_Deny()
    {
        var subject = new CurrentSubjectContext("u1", ["Doctor"], [], null, null, true);
        var policies = new List<AccessPolicy>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Resource = "test_requests",
                Action = "read",
                Effect = AccessPolicyEffect.Allow,
                SubjectType = AccessPolicySubjectType.Role,
                SubjectKey = "Doctor",
                Priority = 10,
                IsEnabled = true,
                Condition = JsonDocument.Parse("""{"field":"doctorId","operator":"eq","value":"@CurrentUserId"}""")
            },
            new()
            {
                Id = Guid.NewGuid(),
                Resource = "test_requests",
                Action = "read",
                Effect = AccessPolicyEffect.Deny,
                SubjectType = AccessPolicySubjectType.Role,
                SubjectKey = "Doctor",
                Priority = 1,
                IsEnabled = true,
                Condition = JsonDocument.Parse("""{"field":"status","operator":"eq","value":"Blocked"}""")
            }
        };

        var evaluator = new AccessPolicyEvaluator(
            new FakeCurrentSubjectAccessor(subject),
            new FakeRuleStore(policies),
            new AccessPolicyConditionParser(),
            new AccessPolicyConditionValidator(),
            new AccessPolicyExpressionCompiler(new AccessPolicyRuntimeTokenResolver()));

        var data = new[]
        {
            new TestRequest { Id = 1, DoctorId = "u1", Status = "Open" },
            new TestRequest { Id = 2, DoctorId = "u1", Status = "Blocked" },
            new TestRequest { Id = 3, DoctorId = "u2", Status = "Open" }
        }.AsQueryable();

        var query = await evaluator.ApplyAsync(data, "test_requests", "read", CancellationToken.None);
        var ids = query.Select(x => x.Id).ToArray();

        Assert.Single(ids);
        Assert.Equal(1, ids[0]);
    }

    [Fact]
    public async Task Evaluator_Should_Grant_Admin_Full_Access_With_Global_Wildcard()
    {
        var subject = new CurrentSubjectContext("admin-1", ["Admin"], [], null, null, true);
        var policies = new List<AccessPolicy>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Resource = "*",
                Action = "*",
                Effect = AccessPolicyEffect.Allow,
                SubjectType = AccessPolicySubjectType.Role,
                SubjectKey = "Admin",
                Priority = 1,
                IsEnabled = true,
                Condition = null
            }
        };

        var evaluator = CreateEvaluator(subject, policies);
        var data = new[]
        {
            new TestRequest { Id = 1, DoctorId = "u1", Status = "Open" },
            new TestRequest { Id = 2, DoctorId = "u2", Status = "Blocked" }
        }.AsQueryable();

        var query = await evaluator.ApplyAsync(data, "any_resource", "any_action", CancellationToken.None);
        var ids = query.Select(x => x.Id).OrderBy(x => x).ToArray();

        Assert.Equal([1, 2], ids);
    }

    [Fact]
    public async Task Evaluator_Should_Match_Wildcard_Resource_With_Exact_Action()
    {
        var subject = new CurrentSubjectContext("u1", ["Doctor"], [], null, null, true);
        var policies = new List<AccessPolicy>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Resource = "*",
                Action = "read",
                Effect = AccessPolicyEffect.Allow,
                SubjectType = AccessPolicySubjectType.Role,
                SubjectKey = "Doctor",
                Priority = 1,
                IsEnabled = true,
                Condition = JsonDocument.Parse("""{"field":"doctorId","operator":"eq","value":"@CurrentUserId"}""")
            }
        };

        var evaluator = CreateEvaluator(subject, policies);
        var data = new[]
        {
            new TestRequest { Id = 1, DoctorId = "u1", Status = "Open" },
            new TestRequest { Id = 2, DoctorId = "u2", Status = "Open" }
        }.AsQueryable();

        var query = await evaluator.ApplyAsync(data, "test_requests", "read", CancellationToken.None);
        var ids = query.Select(x => x.Id).ToArray();

        Assert.Equal([1], ids);
    }

    [Fact]
    public async Task Evaluator_Should_Match_Exact_Resource_With_Wildcard_Action()
    {
        var subject = new CurrentSubjectContext("u1", ["Doctor"], [], null, null, true);
        var policies = new List<AccessPolicy>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Resource = "test_requests",
                Action = "*",
                Effect = AccessPolicyEffect.Allow,
                SubjectType = AccessPolicySubjectType.Role,
                SubjectKey = "Doctor",
                Priority = 1,
                IsEnabled = true,
                Condition = JsonDocument.Parse("""{"field":"doctorId","operator":"eq","value":"@CurrentUserId"}""")
            }
        };

        var evaluator = CreateEvaluator(subject, policies);
        var data = new[]
        {
            new TestRequest { Id = 1, DoctorId = "u1", Status = "Open" },
            new TestRequest { Id = 2, DoctorId = "u2", Status = "Open" }
        }.AsQueryable();

        var query = await evaluator.ApplyAsync(data, "test_requests", "update", CancellationToken.None);
        var ids = query.Select(x => x.Id).ToArray();

        Assert.Equal([1], ids);
    }

    [Fact]
    public async Task Evaluator_Should_Apply_Wildcard_Deny_Over_Allow()
    {
        var subject = new CurrentSubjectContext("u1", ["Doctor"], [], null, null, true);
        var policies = new List<AccessPolicy>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Resource = "test_requests",
                Action = "read",
                Effect = AccessPolicyEffect.Allow,
                SubjectType = AccessPolicySubjectType.Role,
                SubjectKey = "Doctor",
                Priority = 10,
                IsEnabled = true,
                Condition = null
            },
            new()
            {
                Id = Guid.NewGuid(),
                Resource = "*",
                Action = "*",
                Effect = AccessPolicyEffect.Deny,
                SubjectType = AccessPolicySubjectType.Role,
                SubjectKey = "Doctor",
                Priority = 1,
                IsEnabled = true,
                Condition = null
            }
        };

        var evaluator = CreateEvaluator(subject, policies);
        var data = new[] { new TestRequest { Id = 1, DoctorId = "u1", Status = "Open" } }.AsQueryable();

        var query = await evaluator.ApplyAsync(data, "test_requests", "read", CancellationToken.None);
        var ids = query.Select(x => x.Id).ToArray();

        Assert.Empty(ids);
    }

    [Fact]
    public async Task Evaluator_Should_Prefer_Exact_Rule_When_Priority_Is_Equal()
    {
        var subject = new CurrentSubjectContext("u1", ["Doctor"], [], null, null, true);
        var policies = new List<AccessPolicy>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Resource = "*",
                Action = "read",
                Effect = AccessPolicyEffect.Allow,
                SubjectType = AccessPolicySubjectType.Role,
                SubjectKey = "Doctor",
                Priority = 5,
                IsEnabled = true,
                Condition = JsonDocument.Parse("""{"field":"status","operator":"eq","value":"Open"}""")
            },
            new()
            {
                Id = Guid.NewGuid(),
                Resource = "test_requests",
                Action = "read",
                Effect = AccessPolicyEffect.Allow,
                SubjectType = AccessPolicySubjectType.Role,
                SubjectKey = "Doctor",
                Priority = 5,
                IsEnabled = true,
                Condition = JsonDocument.Parse("""{"field":"doctorId","operator":"eq","value":"@CurrentUserId"}""")
            }
        };

        var evaluator = CreateEvaluator(subject, policies);
        var data = new[]
        {
            new TestRequest { Id = 1, DoctorId = "u1", Status = "Closed" },
            new TestRequest { Id = 2, DoctorId = "u2", Status = "Open" }
        }.AsQueryable();

        var query = await evaluator.ApplyAsync(data, "test_requests", "read", CancellationToken.None);
        var ids = query.Select(x => x.Id).OrderBy(x => x).ToArray();

        Assert.Equal([1, 2], ids);
    }

    [Fact]
    public async Task Evaluator_Should_Allow_InactiveViewer_To_List_Only_Inactive_Users()
    {
        var subject = new CurrentSubjectContext("viewer-1", ["InactiveViewer"], [], null, null, true);
        var policies = new List<AccessPolicy>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Resource = "users",
                Action = "read",
                Effect = AccessPolicyEffect.Allow,
                SubjectType = AccessPolicySubjectType.Role,
                SubjectKey = "InactiveViewer",
                Priority = 10,
                IsEnabled = true,
                Condition = JsonDocument.Parse("""{"all":[{"field":"isActive","operator":"eq","value":false}]}""")
            }
        };

        var evaluator = CreateEvaluator(subject, policies);
        var data = new[]
        {
            new User { Id = "u1", FullName = "Inactive A", Email = "i1@test.com", IsActive = false },
            new User { Id = "u2", FullName = "Active B", Email = "a1@test.com", IsActive = true },
            new User { Id = "u3", FullName = "Inactive C", Email = "i2@test.com", IsActive = false }
        }.AsQueryable();

        var query = await evaluator.ApplyAsync(data, "users", "read", CancellationToken.None);
        var ids = query.Select(x => x.Id).OrderBy(x => x).ToArray();

        Assert.Equal(["u1", "u3"], ids);
    }

    [Fact]
    public async Task Evaluator_Should_Not_Return_Active_Users_For_InactiveViewer()
    {
        var subject = new CurrentSubjectContext("viewer-1", ["InactiveViewer"], [], null, null, true);
        var policies = new List<AccessPolicy>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Resource = "users",
                Action = "read",
                Effect = AccessPolicyEffect.Allow,
                SubjectType = AccessPolicySubjectType.Role,
                SubjectKey = "InactiveViewer",
                Priority = 10,
                IsEnabled = true,
                Condition = JsonDocument.Parse("""{"field":"isActive","operator":"eq","value":false}""")
            }
        };

        var evaluator = CreateEvaluator(subject, policies);
        var data = new[] { new User { Id = "u-active", FullName = "Active", Email = "a@test.com", IsActive = true } }.AsQueryable();

        var query = await evaluator.ApplyAsync(data, "users", "read", CancellationToken.None);

        Assert.Empty(query);
    }

    [Fact]
    public async Task Evaluator_Should_Allow_Admin_To_List_All_Users_Via_Db_Policy()
    {
        var subject = new CurrentSubjectContext("admin-1", ["Admin"], [], null, null, true);
        var policies = new List<AccessPolicy>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Resource = "users",
                Action = "read",
                Effect = AccessPolicyEffect.Allow,
                SubjectType = AccessPolicySubjectType.Role,
                SubjectKey = "Admin",
                Priority = 1,
                IsEnabled = true,
                Condition = null
            }
        };

        var evaluator = CreateEvaluator(subject, policies);
        var data = new[]
        {
            new User { Id = "u1", FullName = "Active", Email = "a@test.com", IsActive = true },
            new User { Id = "u2", FullName = "Inactive", Email = "i@test.com", IsActive = false }
        }.AsQueryable();

        var query = await evaluator.ApplyAsync(data, "users", "read", CancellationToken.None);
        var ids = query.Select(x => x.Id).OrderBy(x => x).ToArray();

        Assert.Equal(["u1", "u2"], ids);
    }

    [Fact]
    public async Task Evaluator_Should_Allow_Users_Read_For_NonHardcoded_Role_When_Db_Policy_Exists()
    {
        var subject = new CurrentSubjectContext("viewer-1", ["InactiveViewer"], [], null, null, true);
        var policies = new List<AccessPolicy>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Resource = "users",
                Action = "read",
                Effect = AccessPolicyEffect.Allow,
                SubjectType = AccessPolicySubjectType.Role,
                SubjectKey = "InactiveViewer",
                Priority = 10,
                IsEnabled = true,
                Condition = JsonDocument.Parse("""{"field":"isActive","operator":"eq","value":false}""")
            }
        };

        var evaluator = CreateEvaluator(subject, policies);
        var data = new[] { new User { Id = "u1", FullName = "Inactive", Email = "i@test.com", IsActive = false } }.AsQueryable();

        var query = await evaluator.ApplyAsync(data, "users", "read", CancellationToken.None);
        var ids = query.Select(x => x.Id).ToArray();

        Assert.Equal(["u1"], ids);
    }

    [Fact]
    public async Task Evaluator_Should_Resolve_CurrentUserId_For_ExternalPatient_Ownership()
    {
        var subject = new CurrentSubjectContext("patient-1", ["Patient"], [], null, null, true);
        var policies = new List<AccessPolicy>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Resource = "external_patients",
                Action = "read",
                Effect = AccessPolicyEffect.Allow,
                SubjectType = AccessPolicySubjectType.Role,
                SubjectKey = "Patient",
                Priority = 10,
                IsEnabled = true,
                Condition = JsonDocument.Parse("""{"field":"externalId","operator":"eq","value":"@CurrentUserId"}""")
            }
        };

        var evaluator = CreateEvaluator(subject, policies);
        var data = new[]
        {
            new ExternalPatient { Id = 1, FullName = "Own", ExternalId = "patient-1" },
            new ExternalPatient { Id = 2, FullName = "Other", ExternalId = "patient-2" }
        }.AsQueryable();

        var query = await evaluator.ApplyAsync(data, "external_patients", "read", CancellationToken.None);
        var ids = query.Select(x => x.Id).ToArray();

        Assert.Equal([1], ids);
    }

    [Fact]
    public async Task Evaluator_Should_Resolve_CurrentUserEmail()
    {
        var subject = new CurrentSubjectContext("creator-1", ["Admin"], [], null, null, true, "owner@test.com");
        var policies = new List<AccessPolicy>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Resource = "users",
                Action = "read",
                Effect = AccessPolicyEffect.Allow,
                SubjectType = AccessPolicySubjectType.Role,
                SubjectKey = "Admin",
                Priority = 10,
                IsEnabled = true,
                Condition = JsonDocument.Parse("""{"field":"email","operator":"eq","value":"@CurrentUserEmail"}""")
            }
        };

        var evaluator = CreateEvaluator(subject, policies);
        var data = new[]
        {
            new User { Id = "u1", FullName = "Owner", Email = "owner@test.com" },
            new User { Id = "u2", FullName = "Other", Email = "other@test.com" }
        }.AsQueryable();

        var query = await evaluator.ApplyAsync(data, "users", "read", CancellationToken.None);
        var ids = query.Select(x => x.Id).ToArray();

        Assert.Equal(["u1"], ids);
    }

    [Fact]
    public async Task Evaluator_Should_Filter_Users_Created_By_CurrentUserId()
    {
        var subject = new CurrentSubjectContext("creator-1", ["Admin"], [], null, null, true);
        var policies = new List<AccessPolicy>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Resource = "users",
                Action = "read",
                Effect = AccessPolicyEffect.Allow,
                SubjectType = AccessPolicySubjectType.Role,
                SubjectKey = "Admin",
                Priority = 10,
                IsEnabled = true,
                Condition = JsonDocument.Parse("""{"field":"createdByUserId","operator":"eq","value":"@CurrentUserId"}""")
            }
        };

        var evaluator = CreateEvaluator(subject, policies);
        var data = new[]
        {
            new User { Id = "u1", FullName = "Owned", Email = "owned@test.com", CreatedByUserId = "creator-1" },
            new User { Id = "u2", FullName = "Other", Email = "other@test.com", CreatedByUserId = "creator-2" }
        }.AsQueryable();

        var query = await evaluator.ApplyAsync(data, "users", "read", CancellationToken.None);
        var ids = query.Select(x => x.Id).ToArray();

        Assert.Equal(["u1"], ids);
    }

    private static AccessPolicyEvaluator CreateEvaluator(CurrentSubjectContext subject, IReadOnlyList<AccessPolicy> policies)
    {
        return new AccessPolicyEvaluator(
            new FakeCurrentSubjectAccessor(subject),
            new FakeRuleStore(policies),
            new AccessPolicyConditionParser(),
            new AccessPolicyConditionValidator(),
            new AccessPolicyExpressionCompiler(new AccessPolicyRuntimeTokenResolver()));
    }

    private sealed class FakeCurrentSubjectAccessor(CurrentSubjectContext context) : ICurrentSubjectAccessor
    {
        public Task<CurrentSubjectContext> GetCurrentAsync(CancellationToken cancellationToken) => Task.FromResult(context);
    }

    private sealed class FakeRuleStore(IReadOnlyList<AccessPolicy> policies) : IAccessPolicyRuleStore
    {
        public Task<IReadOnlyList<AccessPolicy>> GetEnabledPoliciesAsync(string resource, string action, CancellationToken cancellationToken)
            => Task.FromResult(
                policies
                    .Where(x =>
                        (x.Resource == resource || x.Resource == "*")
                        && (x.Action == action || x.Action == "*"))
                    .OrderBy(x => x.Priority)
                    .ThenBy(x => x.Resource == resource ? 0 : 1)
                    .ThenBy(x => x.Action == action ? 0 : 1)
                    .ToList() as IReadOnlyList<AccessPolicy>);
    }
}
