using CRM.Medical.Application.Features.Users.Services;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Users.Commands.SendEmailVerification;

public sealed class SendEmailVerificationCommandHandler(
    UserManager<User> userManager,
    IEmailVerificationSender emailVerificationSender) : IRequestHandler<SendEmailVerificationCommand>
{
    public async Task Handle(SendEmailVerificationCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new KeyNotFoundException($"User '{request.UserId}' was not found.");

        if (string.IsNullOrWhiteSpace(user.Email))
            throw new InvalidOperationException("User email is not set.");

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = Uri.EscapeDataString(token);
        var encodedUserId = Uri.EscapeDataString(user.Id);

        var separator = request.ConfirmationBaseUrl.Contains('?', StringComparison.Ordinal) ? "&" : "?";
        var link = $"{request.ConfirmationBaseUrl}{separator}userId={encodedUserId}&token={encodedToken}";

        await emailVerificationSender.SendAsync(user.Email, link, cancellationToken);
    }
}
