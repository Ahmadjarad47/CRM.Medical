using CRM.Medical.Domain.Enums;

namespace CRM.Medical.Application.Features.Permissions.DTOs;

public sealed record AccessPolicyDto(
    Guid Id,
    string Name,
    string Resource,
    string Action,
    SubjectType SubjectType,
    string SubjectId,
    PolicyEffect Effect,
    int Priority,
    string? ConditionJson,
    string? Description,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
