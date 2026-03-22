using CRM.Medical.Application.Features.Users.Services;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Auth.ResendEmailVerification;

public sealed class ResendEmailVerificationCommandHandler(
    UserManager<User> userManager,
    IEmailVerificationSender emailVerificationSender) : IRequestHandler<ResendEmailVerificationCommand>
{
    public async Task Handle(ResendEmailVerificationCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim();
        var user = await userManager.FindByEmailAsync(normalizedEmail);
        if (user is null || user.EmailConfirmed || string.IsNullOrWhiteSpace(user.Email))
            return;

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = Uri.EscapeDataString(token);
        var encodedUserId = Uri.EscapeDataString(user.Id);

        var separator = request.ConfirmationBaseUrl.Contains('?', StringComparison.Ordinal) ? "&" : "?";
        var link = $"{request.ConfirmationBaseUrl}{separator}userId={encodedUserId}&token={encodedToken}";

        await emailVerificationSender.SendAsync(user.Email, link, cancellationToken);
    }
}
