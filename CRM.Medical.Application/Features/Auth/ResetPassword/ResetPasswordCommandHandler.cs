using CRM.Medical.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Auth.ResetPassword;

public sealed class ResetPasswordCommandHandler(UserManager<User> userManager) : IRequestHandler<ResetPasswordCommand>
{
    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim();
        var user = await userManager.FindByEmailAsync(normalizedEmail);
        if (user is null)
        {
            throw new ValidationException(
                [new ValidationFailure(nameof(ResetPasswordCommand.Token), "Reset token is invalid or expired.")]);
        }

        var token = Uri.UnescapeDataString(request.Token);
        var result = await userManager.ResetPasswordAsync(user, token, request.NewPassword);
        if (!result.Succeeded)
        {
            throw new ValidationException(
                [new ValidationFailure(nameof(ResetPasswordCommand.Token), "Reset token is invalid or expired.")]);
        }
    }
}
