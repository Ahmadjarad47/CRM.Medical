using CRM.Medical.Application.Features.Users.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Users.Commands.AddUserPermissions;

public sealed record AddUserPermissionsCommand(
    string UserId,
    IReadOnlyCollection<string> Permissions) : IRequest<UserPermissionsDto>;
