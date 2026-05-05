using CRM.Medical.Domain.Enums;

namespace CRM.Medical.Application.Authorization;

public sealed record AbacPolicyDefinition(
    Guid Id,
    string Name,
    string Resource,
    string Action,
    PolicyEffect Effect,
    SubjectType SubjectType,
    string SubjectId,
    string? Condition,
    int Priority,
    bool IsEnabled,
    string? Description);
