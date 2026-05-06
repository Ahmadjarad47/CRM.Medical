using System.Text.Json;
using CRM.Medical.Domain.Entities;

namespace CRM.Medical.API.Contracts.Admin.AccessPolicies;

public sealed class AccessPolicyUpsertRequest
{
    public string Resource { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public AccessPolicyEffect Effect { get; set; } = AccessPolicyEffect.Allow;
    public AccessPolicySubjectType SubjectType { get; set; } = AccessPolicySubjectType.Role;
    public string SubjectKey { get; set; } = string.Empty;
    public JsonElement? Condition { get; set; }
    public int Priority { get; set; } = 100;
    public bool IsEnabled { get; set; } = true;
    public string? Description { get; set; }
}
