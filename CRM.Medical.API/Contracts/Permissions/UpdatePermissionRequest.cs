using System.Text.Json;
using CRM.Medical.Domain.Enums;

namespace CRM.Medical.API.Contracts.Permissions;

public sealed record UpdatePermissionRequest(
    string Name,
    string Resource,
    string Action,
    SubjectType SubjectType,
    string SubjectId,
    PolicyEffect Effect,
    int Priority,
    JsonDocument? ConditionJson,
    string? Description,
    bool IsEnabled = true);
