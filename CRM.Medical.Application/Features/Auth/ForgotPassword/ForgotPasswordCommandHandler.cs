using CRM.Medical.Application.Features.Users.Services;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Auth.ForgotPassword;

public sealed class ForgotPasswordCommandHandler(
    UserManager<User> userManager,
    IPasswordResetSender passwordResetSender) : IRequestHandler<ForgotPasswordCommand>
{
    public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim();
        var user = await userManager.FindByEmailAsync(normalizedEmail);
        if (user is null || string.IsNullOrWhiteSpace(user.Email) || !user.EmailConfirmed)
            return;

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = Uri.EscapeDataString(token);
        var encodedEmail = Uri.EscapeDataString(user.Email);

        var separator = request.ResetBaseUrl.Contains('?', StringComparison.Ordinal) ? "&" : "?";
        var link = $"{request.ResetBaseUrl}{separator}email={encodedEmail}&token={encodedToken}";

        await passwordResetSender.SendAsync(user.Email, link, cancellationToken);
    }
}
