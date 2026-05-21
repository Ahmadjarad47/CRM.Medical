using CRM.Medical.Application.Authorization;

namespace CRM.Medical.Application.Features.Roles.DTOs;

public sealed record RoleDto(
    string Id,
    string Name,
    IReadOnlyList<AccessPolicySummaryDto> AccessPolicies);
