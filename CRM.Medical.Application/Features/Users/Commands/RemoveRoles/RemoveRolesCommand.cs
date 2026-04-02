using MediatR;

namespace CRM.Medical.Application.Features.Users.Commands.RemoveRoles;

public sealed record RemoveRolesCommand(
    string UserId,
    IReadOnlyList<string> Roles) : IRequest;
