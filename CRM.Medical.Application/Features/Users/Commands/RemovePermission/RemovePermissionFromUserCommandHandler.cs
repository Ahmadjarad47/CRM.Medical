using System.Security.Claims;
using CRM.Medical.Application.Common.Caching;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Users.Commands.RemovePermission;

public sealed class RemovePermissionFromUserCommandHandler(
    UserManager<User> userManager,
    ICacheService cache)
    : IRequestHandler<RemovePermissionFromUserCommand>
{
    public async Task Handle(
        RemovePermissionFromUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new ApplicationNotFoundException($"User '{request.UserId}' not found.");

        var allClaims = await userManager.GetClaimsAsync(user);

        var claimToRemove = allClaims.FirstOrDefault(c =>
            c.Type == UserPermissions.ClaimType &&
            string.Equals(c.Value, request.Permission, StringComparison.OrdinalIgnoreCase));

        if (claimToRemove is null)
            return; // idempotent: nothing to remove

        var result = await userManager.RemoveClaimAsync(user, claimToRemove);
        if (!result.Succeeded)
            throw new ApplicationBadRequestException(
                string.Join("; ", result.Errors.Select(e => e.Description)));

        await cache.RemoveAsync(CacheKeys.UserById(user.Id), cancellationToken);
    }
}
