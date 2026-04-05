using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Features.Permissions.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Permissions.Queries.GetPermissions;

public sealed class GetPermissionsQueryHandler(IPermissionRepository permissions)
    : IRequestHandler<GetPermissionsQuery, IReadOnlyList<PermissionDto>>
{
    public Task<IReadOnlyList<PermissionDto>> Handle(
        GetPermissionsQuery request,
        CancellationToken cancellationToken) =>
        permissions.ListAsync(cancellationToken);
}
