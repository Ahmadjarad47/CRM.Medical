using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Common.Caching;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Application.Features.Users.Services;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Users.Commands.AssignRoles;

public sealed class AssignRolesCommandHandler(
    UserManager<User> userManager,
    ICacheService cache,
    IUserManagementAccess userManagementAccess,
    ICurrentUserAccessor currentUser)
    : IRequestHandler<AssignRolesCommand>
{
    public async Task Handle(AssignRolesCommand request, CancellationToken cancellationToken)
    {
        var actorId = currentUser.GetRequiredUserId();

        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new ApplicationNotFoundException($"User '{request.UserId}' not found.");

        await userManagementAccess.EnsureActorCanManageUserAsync(actorId, user, cancellationToken);

        var actor = await userManager.FindByIdAsync(actorId)
            ?? throw new ApplicationUnauthorizedException("Unable to identify the current user.");

        if (!await userManager.IsInRoleAsync(actor, UserRoles.Admin))
        {
            var disallowed = request.Roles
                .Where(r =>
                    !string.Equals(r, UserRoles.Patient, StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(r, UserRoles.User, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (disallowed.Count > 0)
                throw new ApplicationForbiddenException(
                    "You may only assign the Patient or User role.");
        }

        var currentRoles = await userManager.GetRolesAsync(user);
        var rolesToAdd = request.Roles
            .Except(currentRoles, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (rolesToAdd.Count == 0)
            return;

        var result = await userManager.AddToRolesAsync(user, rolesToAdd);
        if (!result.Succeeded)
            throw new ApplicationBadRequestException(
                string.Join("; ", result.Errors.Select(e => e.Description)));

        await cache.RemoveAsync(CacheKeys.UserById(user.Id), cancellationToken);
    }
}
