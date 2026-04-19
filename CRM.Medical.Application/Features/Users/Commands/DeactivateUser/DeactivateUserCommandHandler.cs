using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Common.Caching;
using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Users.Services;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Users.Commands.DeactivateUser;

public sealed class DeactivateUserCommandHandler(
    UserManager<User> userManager,
    IDateTimeProvider dateTimeProvider,
    ICacheService cache,
    IUserManagementAccess userManagementAccess,
    ICurrentUserAccessor currentUser)
    : IRequestHandler<DeactivateUserCommand>
{
    public async Task Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var actorId = currentUser.GetRequiredUserId();

        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new ApplicationNotFoundException($"User '{request.UserId}' not found.");

        await userManagementAccess.EnsureActorCanManageUserAsync(actorId, user, cancellationToken);

        if (!user.IsActive)
            return;

        user.IsActive = false;
        user.UpdatedAt = dateTimeProvider.UtcNow;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new ApplicationBadRequestException(
                string.Join("; ", result.Errors.Select(e => e.Description)));

        await cache.RemoveAsync(CacheKeys.UserById(user.Id), cancellationToken);
    }
}
