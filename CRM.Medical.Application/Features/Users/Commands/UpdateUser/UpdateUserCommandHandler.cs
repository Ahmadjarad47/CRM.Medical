using CRM.Medical.Application.Features.Users.Common;
using CRM.Medical.Application.Features.Users.DTOs;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Users.Commands.UpdateUser;

public sealed class UpdateUserCommandHandler(UserManager<User> userManager)
    : IRequestHandler<UpdateUserCommand, UserDetailDto>
{
    public async Task<UserDetailDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new KeyNotFoundException($"User '{request.UserId}' was not found.");

        user.DisplayName = request.DisplayName.Trim();

        var normalizedEmail = request.Email.Trim();
        if (!string.Equals(user.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase))
        {
            var setEmailResult = await userManager.SetEmailAsync(user, normalizedEmail);
            setEmailResult.ThrowIfFailed(nameof(request.Email));

            var setUserNameResult = await userManager.SetUserNameAsync(user, normalizedEmail);
            setUserNameResult.ThrowIfFailed(nameof(request.Email));
        }

        var updateResult = await userManager.UpdateAsync(user);
        updateResult.ThrowIfFailed(nameof(UpdateUserCommand));

        return user.ToDetailDto();
    }
}
