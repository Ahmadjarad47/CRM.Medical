using System.Security.Claims;
using CRM.Medical.Application.Common.Caching;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Users.Commands.AssignPermissions;

public sealed class AssignPermissionsToUserCommandHandler(
    UserManager<User> userManager,
    ICacheService cache)
    : IRequestHandler<AssignPermissionsToUserCommand>
{
    public async Task Handle(
        AssignPermissionsToUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new ApplicationNotFoundException($"User '{request.UserId}' not found.");

        var existing = await userManager.GetClaimsAsync(user);
        var existingPermissions = existing
            .Where(c => c.Type == UserPermissions.ClaimType)
            .Select(c => c.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var toAdd = request.Permissions
            .Where(p => !existingPermissions.Contains(p))
            .Select(p => new Claim(UserPermissions.ClaimType, p))
            .ToList();

        if (toAdd.Count == 0)
            return;

        var result = await userManager.AddClaimsAsync(user, toAdd);
        if (!result.Succeeded)
            throw new ApplicationBadRequestException(
                string.Join("; ", result.Errors.Select(e => e.Description)));

        await cache.RemoveAsync(CacheKeys.UserById(user.Id), cancellationToken);
    }
}
