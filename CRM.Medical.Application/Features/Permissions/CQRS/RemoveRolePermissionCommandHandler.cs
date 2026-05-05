using CRM.Medical.Application.Features.Permissions.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Permissions.CQRS;

public sealed class RemoveRolePermissionCommandHandler(IRolePermissionService rolePermissions)
    : IRequestHandler<RemoveRolePermissionCommand, Unit>
{
    public async Task<Unit> Handle(RemoveRolePermissionCommand request, CancellationToken cancellationToken)
    {
        await rolePermissions.RemovePermissionFromRoleAsync(request.RoleId, request.PolicyId, cancellationToken);
        return Unit.Value;
    }
}
