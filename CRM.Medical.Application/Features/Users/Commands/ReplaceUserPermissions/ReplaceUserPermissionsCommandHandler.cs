using System.Security.Claims;
using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Common.Caching;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Application.Features.Users.Services;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Users.Commands.ReplaceUserPermissions;

public sealed class ReplaceUserPermissionsCommandHandler(
    UserManager<User> userManager,
    ICacheService cache,
    IUserManagementAccess userManagementAccess,
    ICurrentUserAccessor currentUser)
    : IRequestHandler<ReplaceUserPermissionsCommand>
{
    public async Task Handle(ReplaceUserPermissionsCommand request, CancellationToken cancellationToken)
    {
        var actorId = currentUser.GetRequiredUserId();

        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new ApplicationNotFoundException($"User '{request.UserId}' not found.");

        await userManagementAccess.EnsureActorCanManageUserAsync(actorId, user, cancellationToken);

        var existing = await userManager.GetClaimsAsync(user);
        var permissionClaims = existing
            .Where(c => c.Type == UserPermissions.ClaimType)
            .ToList();

        if (permissionClaims.Count > 0)
        {
            var removeResult = await userManager.RemoveClaimsAsync(user, permissionClaims);
            if (!removeResult.Succeeded)
                throw new ApplicationBadRequestException(
                    string.Join("; ", removeResult.Errors.Select(e => e.Description)));
        }

        if (request.Permissions.Count > 0)
        {
            var toAdd = request.Permissions
                .Select(p => new Claim(UserPermissions.ClaimType, p))
                .ToList();

            var addResult = await userManager.AddClaimsAsync(user, toAdd);
            if (!addResult.Succeeded)
                throw new ApplicationBadRequestException(
                    string.Join("; ", addResult.Errors.Select(e => e.Description)));
        }

        await cache.RemoveAsync(CacheKeys.UserById(user.Id), cancellationToken);
    }
}
