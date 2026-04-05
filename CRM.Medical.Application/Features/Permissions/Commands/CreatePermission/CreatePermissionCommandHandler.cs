using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Features.Permissions.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Permissions.Commands.CreatePermission;

public sealed class CreatePermissionCommandHandler(
    IPermissionRepository permissions,
    IDateTimeProvider clock)
    : IRequestHandler<CreatePermissionCommand, PermissionDto>
{
    public Task<PermissionDto> Handle(
        CreatePermissionCommand request,
        CancellationToken cancellationToken) =>
        permissions.CreateAsync(request.Name, request.Description, clock.UtcNow, cancellationToken);
}
