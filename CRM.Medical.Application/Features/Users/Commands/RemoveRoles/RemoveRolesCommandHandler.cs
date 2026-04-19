using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Common.Caching;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Users.Services;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Users.Commands.RemoveRoles;

public sealed class RemoveRolesCommandHandler(
    UserManager<User> userManager,
    ICacheService cache,
    IUserManagementAccess userManagementAccess,
    ICurrentUserAccessor currentUser)
    : IRequestHandler<RemoveRolesCommand>
{
    public async Task Handle(RemoveRolesCommand request, CancellationToken cancellationToken)
    {
        var actorId = currentUser.GetRequiredUserId();

        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new ApplicationNotFoundException($"User '{request.UserId}' not found.");

        await userManagementAccess.EnsureActorCanManageUserAsync(actorId, user, cancellationToken);

        var currentRoles = await userManager.GetRolesAsync(user);
        var rolesToRemove = request.Roles
            .Intersect(currentRoles, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (rolesToRemove.Count == 0)
            return;

        var result = await userManager.RemoveFromRolesAsync(user, rolesToRemove);
        if (!result.Succeeded)
            throw new ApplicationBadRequestException(
                string.Join("; ", result.Errors.Select(e => e.Description)));

        await cache.RemoveAsync(CacheKeys.UserById(user.Id), cancellationToken);
    }
}
