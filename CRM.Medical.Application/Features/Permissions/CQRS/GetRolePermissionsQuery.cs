using CRM.Medical.Application.Features.Permissions.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Permissions.CQRS;

public sealed record GetRolePermissionsQuery(string RoleId) : IRequest<IReadOnlyList<AccessPolicyDto>>;
