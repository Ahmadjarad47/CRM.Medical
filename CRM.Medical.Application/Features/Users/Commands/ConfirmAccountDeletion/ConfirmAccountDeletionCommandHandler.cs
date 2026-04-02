using CRM.Medical.Application.Common.Caching;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Users.Commands.RequestAccountDeletion;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Users.Commands.ConfirmAccountDeletion;

public sealed class ConfirmAccountDeletionCommandHandler(
    UserManager<User> userManager,
    ICacheService cache)
    : IRequestHandler<ConfirmAccountDeletionCommand>
{
    public async Task Handle(
        ConfirmAccountDeletionCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email)
            ?? throw new ApplicationBadRequestException("Invalid account deletion request.");

        var isValid = await userManager.VerifyUserTokenAsync(
            user,
            RequestAccountDeletionCommandHandler.TokenProvider,
            RequestAccountDeletionCommandHandler.Purpose,
            request.Token);

        if (!isValid)
            throw new ApplicationBadRequestException("Invalid or expired deletion token.");

        // Invalidate cache before deletion
        await cache.RemoveAsync(CacheKeys.UserById(user.Id), cancellationToken);

        // Identity cascades to its own tables; our RefreshTokens table has ON DELETE CASCADE
        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
            throw new ApplicationBadRequestException(
                string.Join("; ", result.Errors.Select(e => e.Description)));
    }
}
