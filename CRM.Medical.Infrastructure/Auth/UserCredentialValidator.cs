using CRM.Medical.Application.Auth;
using CRM.Medical.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Infrastructure.Auth;

public sealed class UserCredentialValidator(UserManager<User> userManager) : IUserCredentialValidator
{
    public async Task<AuthenticatedUser?> ValidateAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null || !await userManager.CheckPasswordAsync(user, password))
            return null;

        var resolvedEmail = user.Email ?? email;
        return new AuthenticatedUser(user.Id, resolvedEmail, user.DisplayName);
    }
}
