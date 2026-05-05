using System.Text.Json;
using CRM.Medical.Domain.Enums;
using CRM.Medical.Application.Features.Permissions.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Permissions.CQRS;

public sealed record CreateAccessPolicyCommand(
    string Name,
    string Resource,
    string Action,
    SubjectType SubjectType,
    string SubjectId,
    PolicyEffect Effect,
    int Priority,
    JsonDocument? ConditionJson,
    string? Description,
    bool IsEnabled) : IRequest<AccessPolicyDto>;
