using CRM.Medical.Application.Features.Roles.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Roles.Queries.GetRoleById;

public sealed record GetRoleByIdQuery(string Id) : IRequest<RoleDto>;
