using CRM.Medical.Application.Features.Users.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Users.Commands.UpdateUserPermissions;

public sealed record UpdateUserPermissionsCommand(
    string UserId,
    IReadOnlyCollection<string> Permissions) : IRequest<UserPermissionsDto>;
