using CRM.Medical.Application.Features.Users.Common;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Users.Commands.UnlockUser;

public sealed class UnlockUserCommandHandler(UserManager<User> userManager)
    : IRequestHandler<UnlockUserCommand>
{
    public async Task Handle(UnlockUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new KeyNotFoundException($"User '{request.UserId}' was not found.");

        var unlockResult = await userManager.SetLockoutEndDateAsync(user, null);
        unlockResult.ThrowIfFailed(nameof(UnlockUserCommand));

        var resetResult = await userManager.ResetAccessFailedCountAsync(user);
        resetResult.ThrowIfFailed(nameof(UnlockUserCommand));
    }
}
