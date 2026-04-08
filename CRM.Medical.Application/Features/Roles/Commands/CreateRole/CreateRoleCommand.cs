using CRM.Medical.Application.Features.Roles.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Roles.Commands.CreateRole;

public sealed record CreateRoleCommand(string Name) : IRequest<RoleDto>;
