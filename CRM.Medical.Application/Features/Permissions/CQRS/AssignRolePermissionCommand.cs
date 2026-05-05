using System.Text.Json;
using CRM.Medical.Domain.Enums;
using MediatR;

namespace CRM.Medical.Application.Features.Permissions.CQRS;

public sealed record AssignRolePermissionCommand(
    string RoleId,
    string Name,
    string Resource,
    string Action,
    PolicyEffect Effect,
    int Priority,
    JsonDocument? ConditionJson,
    string? Description,
    bool IsEnabled) : IRequest<Unit>;
