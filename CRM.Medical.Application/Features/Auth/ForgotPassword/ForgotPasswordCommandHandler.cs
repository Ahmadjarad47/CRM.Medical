using CRM.Medical.Application.Features.Users.Services;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Auth.ForgotPassword;

public sealed class ForgotPasswordCommandHandler(
    UserManager<User> userManager,
    IPasswordResetSender passwordResetSender)
    : IRequestHandler<ForgotPasswordCommand>
{
    public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        // Always return success — never reveal whether an account exists (prevents enumeration)
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || !user.EmailConfirmed || !user.IsActive)
            return;

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        await passwordResetSender.SendAsync(user.Email!, token, cancellationToken);
    }
}
