using System.Security.Claims;
using CRM.Medical.Application.Features.Users.Common;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Application.Features.Users.DTOs;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Users.Commands.UpdateUserPermissions;

public sealed class UpdateUserPermissionsCommandHandler(UserManager<User> userManager)
    : IRequestHandler<UpdateUserPermissionsCommand, UserPermissionsDto>
{
    public async Task<UserPermissionsDto> Handle(UpdateUserPermissionsCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new KeyNotFoundException($"User '{request.UserId}' was not found.");

        var claims = await userManager.GetClaimsAsync(user);
        var permissionClaims = claims
            .Where(x => x.Type == UserPermissionClaimTypes.Permission)
            .ToArray();

        foreach (var claim in permissionClaims)
        {
            var removeResult = await userManager.RemoveClaimAsync(user, claim);
            removeResult.ThrowIfFailed(nameof(UpdateUserPermissionsCommand));
        }

        var normalizedPermissions = request.Permissions
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        foreach (var permission in normalizedPermissions)
        {
            var addResult = await userManager.AddClaimAsync(user, new Claim(UserPermissionClaimTypes.Permission, permission));
            addResult.ThrowIfFailed(nameof(UpdateUserPermissionsCommand));
        }

        return new UserPermissionsDto(user.Id, normalizedPermissions.OrderBy(x => x).ToArray());
    }
}
