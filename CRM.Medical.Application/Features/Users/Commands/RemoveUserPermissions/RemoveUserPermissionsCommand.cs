using CRM.Medical.Application.Features.Users.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Users.Commands.RemoveUserPermissions;

public sealed record RemoveUserPermissionsCommand(
    string UserId,
    IReadOnlyCollection<string> Permissions) : IRequest<UserPermissionsDto>;
