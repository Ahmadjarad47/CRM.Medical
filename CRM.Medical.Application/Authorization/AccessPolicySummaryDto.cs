using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Authorization;

public sealed record AccessPolicySummaryDto(
    Guid Id,
    string Resource,
    string Action,
    AccessPolicyEffect Effect,
    AccessPolicySubjectType SubjectType,
    string SubjectKey,
    int Priority,
    bool IsEnabled,
    string? Description,
    DateTime? ValidFrom,
    DateTime? ValidTo);
