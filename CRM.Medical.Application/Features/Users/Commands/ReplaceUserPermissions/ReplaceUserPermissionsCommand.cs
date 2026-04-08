using MediatR;

namespace CRM.Medical.Application.Features.Users.Commands.ReplaceUserPermissions;

public sealed record ReplaceUserPermissionsCommand(string UserId, IReadOnlyList<string> Permissions) : IRequest;
