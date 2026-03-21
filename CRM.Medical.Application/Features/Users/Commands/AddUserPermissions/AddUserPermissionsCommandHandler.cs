using System.Security.Claims;
using CRM.Medical.Application.Features.Users.Common;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Application.Features.Users.DTOs;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Users.Commands.AddUserPermissions;

public sealed class AddUserPermissionsCommandHandler(UserManager<User> userManager)
    : IRequestHandler<AddUserPermissionsCommand, UserPermissionsDto>
{
    public async Task<UserPermissionsDto> Handle(AddUserPermissionsCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new KeyNotFoundException($"User '{request.UserId}' was not found.");

        var existingClaims = await userManager.GetClaimsAsync(user);
        var existingPermissions = existingClaims
            .Where(x => x.Type == UserPermissionClaimTypes.Permission)
            .Select(x => x.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var toAdd = request.Permissions
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Where(x => !existingPermissions.Contains(x))
            .ToArray();

        foreach (var permission in toAdd)
        {
            var addResult = await userManager.AddClaimAsync(user, new Claim(UserPermissionClaimTypes.Permission, permission));
            addResult.ThrowIfFailed(nameof(AddUserPermissionsCommand));
        }

        var allPermissions = existingPermissions
            .Union(toAdd, StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x)
            .ToArray();

        return new UserPermissionsDto(user.Id, allPermissions);
    }
}
