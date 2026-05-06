using CRM.Medical.Domain.Enums;

namespace CRM.Medical.Domain.Entities;

/// <summary>
/// Dynamic ABAC policy entry persisted in the database.
/// Policy is keyed by Resource + Action (for example TestResult:View in authorization policy names).
/// </summary>
public sealed class AccessPolicy : BaseEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Resource { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    public PolicyEffect Effect { get; set; } = PolicyEffect.Allow;

    public SubjectType SubjectType { get; set; } = SubjectType.Role;

    public string SubjectId { get; set; } = string.Empty;

    /// <summary>
    /// JSON-encoded condition that is compiled and cached by the policy engine.
    /// Null/empty means unconditional allow.
    /// </summary>
    public string? Condition { get; set; }

    /// <summary>
    /// Higher numbers are evaluated first.
    /// </summary>
    public int Priority { get; set; } = 100;

    public bool IsEnabled { get; set; } = true;

    public string? Description { get; set; }
}
