using System.Text.Json;
using CRM.Medical.Domain.Enums;
using MediatR;

namespace CRM.Medical.Application.Features.Permissions.CQRS;

public sealed record UpdateAccessPolicyCommand(
    Guid Id,
    string Name,
    string Resource,
    string Action,
    SubjectType SubjectType,
    string SubjectId,
    PolicyEffect Effect,
    int Priority,
    JsonDocument? ConditionJson,
    string? Description,
    bool IsEnabled) : IRequest<Unit>;
