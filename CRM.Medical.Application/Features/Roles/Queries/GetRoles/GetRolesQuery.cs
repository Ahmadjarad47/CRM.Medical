using CRM.Medical.Application.Features.Roles.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Roles.Queries.GetRoles;

public sealed record GetRolesQuery : IRequest<IReadOnlyList<RoleDto>>;
