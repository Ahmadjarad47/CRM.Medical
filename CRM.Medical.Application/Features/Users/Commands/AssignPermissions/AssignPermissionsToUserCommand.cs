using MediatR;

namespace CRM.Medical.Application.Features.Users.Commands.AssignPermissions;

public sealed record AssignPermissionsToUserCommand(
    string UserId,
    IReadOnlyList<string> Permissions) : IRequest;
