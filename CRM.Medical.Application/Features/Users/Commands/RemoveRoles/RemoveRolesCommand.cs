using CRM.Medical.Application.Features.Users.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Users.Commands.RemoveRoles;

public sealed record RemoveRolesCommand(
    string UserId,
    IReadOnlyCollection<string> Roles) : IRequest<UserRolesDto>;
