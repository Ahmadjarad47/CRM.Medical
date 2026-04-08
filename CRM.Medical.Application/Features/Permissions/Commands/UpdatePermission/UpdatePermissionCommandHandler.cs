using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Exceptions;
using MediatR;

namespace CRM.Medical.Application.Features.Permissions.Commands.UpdatePermission;

public sealed class UpdatePermissionCommandHandler(IPermissionRepository permissions)
    : IRequestHandler<UpdatePermissionCommand>
{
    public async Task Handle(UpdatePermissionCommand request, CancellationToken cancellationToken)
    {
        var existing = await permissions.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Permission '{request.Id}' was not found.");

        await permissions.UpdateDescriptionAsync(existing.Id, request.Description, cancellationToken);
    }
}
