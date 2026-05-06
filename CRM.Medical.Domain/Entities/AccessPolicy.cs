using System.Text.Json;

namespace CRM.Medical.Domain.Entities;

public enum AccessPolicyEffect
{
    Deny = 0,
    Allow = 1
}

public enum AccessPolicySubjectType
{
    User = 0,
    Role = 1,
    Authenticated = 2,
    All = 3
}

public sealed class AccessPolicy : BaseEntity
{
    public Guid Id { get; set; }
    public string Resource { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public AccessPolicyEffect Effect { get; set; } = AccessPolicyEffect.Allow;
    public AccessPolicySubjectType SubjectType { get; set; } = AccessPolicySubjectType.Role;
    public string SubjectKey { get; set; } = string.Empty;
    public JsonDocument? Condition { get; set; }
    public int Priority { get; set; }
    public bool IsEnabled { get; set; } = true;
    public string? Description { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? UpdatedByUserId { get; set; }
}
