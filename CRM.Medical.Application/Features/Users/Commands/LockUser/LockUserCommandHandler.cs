using CRM.Medical.Application.Features.Users.Common;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Users.Commands.LockUser;

public sealed class LockUserCommandHandler(UserManager<User> userManager)
    : IRequestHandler<LockUserCommand>
{
    public async Task Handle(LockUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new KeyNotFoundException($"User '{request.UserId}' was not found.");

        user.LockoutEnabled = true;

        var lockoutEnd = DateTimeOffset.UtcNow.AddMinutes(request.LockoutMinutes);
        var lockResult = await userManager.SetLockoutEndDateAsync(user, lockoutEnd);
        lockResult.ThrowIfFailed(nameof(LockUserCommand));
    }
}
