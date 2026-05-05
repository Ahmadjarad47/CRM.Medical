using CRM.Medical.Application.Features.Permissions.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Permissions.CQRS;

public sealed class AssignRolePermissionCommandHandler(IRolePermissionService rolePermissions)
    : IRequestHandler<AssignRolePermissionCommand, Unit>
{
    public async Task<Unit> Handle(AssignRolePermissionCommand request, CancellationToken cancellationToken)
    {
        await rolePermissions.AssignPermissionToRoleAsync(
            request.RoleId,
            request.Name,
            request.Resource,
            request.Action,
            request.Effect,
            request.Priority,
            request.ConditionJson,
            request.Description,
            request.IsEnabled,
            cancellationToken);
        return Unit.Value;
    }
}
