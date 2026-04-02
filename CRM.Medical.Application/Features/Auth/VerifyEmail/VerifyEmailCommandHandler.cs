using CRM.Medical.Application.Exceptions;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Auth.VerifyEmail;

public sealed class VerifyEmailCommandHandler(UserManager<User> userManager)
    : IRequestHandler<VerifyEmailCommand>
{
    public async Task Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email)
            ?? throw new ApplicationBadRequestException("Invalid email verification request.");

        if (user.EmailConfirmed)
            return; // idempotent

        var result = await userManager.ConfirmEmailAsync(user, request.Token);
        if (!result.Succeeded)
            throw new ApplicationBadRequestException(
                string.Join("; ", result.Errors.Select(e => e.Description)));
    }
}
