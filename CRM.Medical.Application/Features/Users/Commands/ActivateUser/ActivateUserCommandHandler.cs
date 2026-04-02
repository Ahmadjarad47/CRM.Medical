using CRM.Medical.Application.Common.Caching;
using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Users.Commands.ActivateUser;

public sealed class ActivateUserCommandHandler(
    UserManager<User> userManager,
    IDateTimeProvider dateTimeProvider,
    ICacheService cache)
    : IRequestHandler<ActivateUserCommand>
{
    public async Task Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new ApplicationNotFoundException($"User '{request.UserId}' not found.");

        if (user.IsActive)
            return;

        user.IsActive = true;
        user.UpdatedAt = dateTimeProvider.UtcNow;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new ApplicationBadRequestException(
                string.Join("; ", result.Errors.Select(e => e.Description)));

        await cache.RemoveAsync(CacheKeys.UserById(user.Id), cancellationToken);
    }
}
