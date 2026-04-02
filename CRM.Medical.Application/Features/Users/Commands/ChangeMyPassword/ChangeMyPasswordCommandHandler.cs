using CRM.Medical.Application.Exceptions;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Users.Commands.ChangeMyPassword;

public sealed class ChangeMyPasswordCommandHandler(UserManager<User> userManager)
    : IRequestHandler<ChangeMyPasswordCommand>
{
    public async Task Handle(ChangeMyPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new ApplicationNotFoundException("User not found.");

        var result = await userManager.ChangePasswordAsync(
            user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
            throw new ApplicationBadRequestException(
                string.Join("; ", result.Errors.Select(e => e.Description)));
    }
}
