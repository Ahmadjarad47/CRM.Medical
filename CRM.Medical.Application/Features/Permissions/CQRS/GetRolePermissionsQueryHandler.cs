using CRM.Medical.Application.Features.Permissions.DTOs;
using CRM.Medical.Application.Features.Permissions.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Permissions.CQRS;

public sealed class GetRolePermissionsQueryHandler(IRolePermissionService rolePermissions)
    : IRequestHandler<GetRolePermissionsQuery, IReadOnlyList<AccessPolicyDto>>
{
    public Task<IReadOnlyList<AccessPolicyDto>> Handle(
        GetRolePermissionsQuery request,
        CancellationToken cancellationToken) =>
        rolePermissions.GetRolePermissionsAsync(request.RoleId, cancellationToken);
}
