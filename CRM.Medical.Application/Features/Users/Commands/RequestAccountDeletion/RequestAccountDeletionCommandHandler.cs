using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Users.Services;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Users.Commands.RequestAccountDeletion;

public sealed class RequestAccountDeletionCommandHandler(
    UserManager<User> userManager,
    IAccountDeletionSender accountDeletionSender)
    : IRequestHandler<RequestAccountDeletionCommand>
{
    internal const string TokenProvider = "Default";
    internal const string Purpose = "AccountDeletion";

    public async Task Handle(
        RequestAccountDeletionCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new ApplicationNotFoundException("User not found.");

        var token = await userManager.GenerateUserTokenAsync(user, TokenProvider, Purpose);
        await accountDeletionSender.SendAsync(user.Email!, token, cancellationToken);
    }
}
