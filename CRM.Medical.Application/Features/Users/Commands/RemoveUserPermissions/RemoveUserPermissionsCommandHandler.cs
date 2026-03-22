using CRM.Medical.Application.Features.Users.Common;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Application.Features.Users.DTOs;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Users.Commands.RemoveUserPermissions;

public sealed class RemoveUserPermissionsCommandHandler(UserManager<User> userManager)
    : IRequestHandler<RemoveUserPermissionsCommand, UserPermissionsDto>
{
    public async Task<UserPermissionsDto> Handle(RemoveUserPermissionsCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new KeyNotFoundException($"User '{request.UserId}' was not found.");

        var toRemove = request.Permissions
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var claims = await userManager.GetClaimsAsync(user);
        var permissionClaims = claims
            .Where(x => x.Type == UserPermissionClaimTypes.Permission && toRemove.Contains(x.Value))
            .ToArray();

        foreach (var claim in permissionClaims)
        {
            var removeResult = await userManager.RemoveClaimAsync(user, claim);
            removeResult.ThrowIfFailed(nameof(RemoveUserPermissionsCommand));
        }

        var remainingPermissions = claims
            .Where(x => x.Type == UserPermissionClaimTypes.Permission && !toRemove.Contains(x.Value))
            .Select(x => x.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x)
            .ToArray();

        return user.ToPermissionsDto(remainingPermissions);
    }
}
