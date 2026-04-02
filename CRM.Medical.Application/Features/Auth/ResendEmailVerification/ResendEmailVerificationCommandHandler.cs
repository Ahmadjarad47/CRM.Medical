using CRM.Medical.Application.Features.Users.Services;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Auth.ResendEmailVerification;

public sealed class ResendEmailVerificationCommandHandler(
    UserManager<User> userManager,
    IEmailVerificationSender emailSender)
    : IRequestHandler<ResendEmailVerificationCommand>
{
    public async Task Handle(
        ResendEmailVerificationCommand request,
        CancellationToken cancellationToken)
    {
        // Always return success to prevent account enumeration
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || user.EmailConfirmed)
            return;

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        await emailSender.SendAsync(user.Email!, token, cancellationToken);
    }
}
