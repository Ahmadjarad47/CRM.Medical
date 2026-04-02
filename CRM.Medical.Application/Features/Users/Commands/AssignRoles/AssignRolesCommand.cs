using MediatR;

namespace CRM.Medical.Application.Features.Users.Commands.AssignRoles;

public sealed record AssignRolesCommand(
    string UserId,
    IReadOnlyList<string> Roles) : IRequest;
