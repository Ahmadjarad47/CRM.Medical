using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Permissions.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Permissions.Queries.GetPermissionById;

public sealed class GetPermissionByIdQueryHandler(IPermissionRepository permissions)
    : IRequestHandler<GetPermissionByIdQuery, PermissionDto>
{
    public async Task<PermissionDto> Handle(
        GetPermissionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var row = await permissions.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Permission '{request.Id}' was not found.");

        return row;
    }
}
