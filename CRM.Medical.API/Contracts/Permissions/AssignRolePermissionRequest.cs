using System.Text.Json;
using CRM.Medical.Domain.Enums;

namespace CRM.Medical.API.Contracts.Permissions;

public sealed record AssignRolePermissionRequest(
    string Name,
    string Resource,
    string Action,
    PolicyEffect Effect,
    int Priority,
    JsonDocument? ConditionJson,
    string? Description,
    bool IsEnabled = true);
