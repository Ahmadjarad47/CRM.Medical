using CRM.Medical.Application.Features.Users.Common;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Users.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommandHandler(UserManager<User> userManager)
    : IRequestHandler<ConfirmEmailCommand>
{
    public async Task Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new KeyNotFoundException($"User '{request.UserId}' was not found.");

        var result = await userManager.ConfirmEmailAsync(user, request.Token);
        result.ThrowIfFailed(nameof(ConfirmEmailCommand));
    }
}
