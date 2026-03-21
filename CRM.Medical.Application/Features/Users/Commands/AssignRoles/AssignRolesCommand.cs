using CRM.Medical.Application.Features.Users.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Users.Commands.AssignRoles;

public sealed record AssignRolesCommand(
    string UserId,
    IReadOnlyCollection<string> Roles) : IRequest<UserRolesDto>;
