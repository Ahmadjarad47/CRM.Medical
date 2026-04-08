using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Common.Caching;
using CRM.Medical.Application.Exceptions;
using MediatR;

namespace CRM.Medical.Application.Features.Permissions.Commands.DeletePermission;

public sealed class DeletePermissionCommandHandler(
    IPermissionRepository permissions,
    ICacheService cache)
    : IRequestHandler<DeletePermissionCommand>
{
    public async Task Handle(DeletePermissionCommand request, CancellationToken cancellationToken)
    {
        var outcome = await permissions.DeleteAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Permission '{request.Id}' was not found.");

        foreach (var userId in outcome.UserIdsForCacheInvalidation)
            await cache.RemoveAsync(CacheKeys.UserById(userId), cancellationToken);
    }
}
