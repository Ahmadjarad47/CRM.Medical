using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Application.Features.Users.Services;
using CRM.Medical.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Infrastructure.Persistence;

public sealed class UserManagementAccessService(UserManager<User> userManager)
    : IUserManagementAccess
{
    public async Task EnsureActorCanCreateUsersAsync(
        string actorUserId,
        CancellationToken cancellationToken = default)
    {
        var actor = await userManager.FindByIdAsync(actorUserId)
            ?? throw new ApplicationUnauthorizedException("Unable to identify the current user.");

        if (await userManager.IsInRoleAsync(actor, UserRoles.Admin))
            return;

        if (await userManager.IsInRoleAsync(actor, UserRoles.Doctor))
            return;

        if (await userManager.IsInRoleAsync(actor, UserRoles.LabPartner))
            return;

        throw new ApplicationForbiddenException(
            "Only administrators, doctors, and lab partners can create users.");
    }

    public async Task EnsureActorCanManageUserAsync(
        string actorUserId,
        User targetUser,
        CancellationToken cancellationToken = default)
    {
        if (string.Equals(actorUserId, targetUser.Id, StringComparison.Ordinal))
            return;

        var actor = await userManager.FindByIdAsync(actorUserId)
            ?? throw new ApplicationUnauthorizedException("Unable to identify the current user.");

        if (await userManager.IsInRoleAsync(actor, UserRoles.Admin))
            return;

        if (await userManager.IsInRoleAsync(actor, UserRoles.Doctor))
        {
            if (IsCreatedBy(actorUserId, targetUser))
                return;

            throw new ApplicationForbiddenException(
                "You can only manage users you created.");
        }

        if (await userManager.IsInRoleAsync(actor, UserRoles.LabPartner))
        {
            if (IsCreatedBy(actorUserId, targetUser))
                return;

            throw new ApplicationForbiddenException(
                "You can only manage users you created.");
        }

        throw new ApplicationForbiddenException("You are not allowed to manage this user.");
    }

    private static bool IsCreatedBy(string actorUserId, User targetUser) =>
        string.Equals(targetUser.CreatedByUserId, actorUserId, StringComparison.Ordinal);
}
