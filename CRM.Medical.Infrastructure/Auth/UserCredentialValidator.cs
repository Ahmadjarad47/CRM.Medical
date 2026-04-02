using CRM.Medical.Application.Auth;
using CRM.Medical.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Infrastructure.Auth;

public sealed class UserCredentialValidator(UserManager<User> userManager) : IUserCredentialValidator
{
    public async Task<User?> ValidateAsync(string email, string password, CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return null;

        if (await userManager.IsLockedOutAsync(user))
            return null;

        var passwordValid = await userManager.CheckPasswordAsync(user, password);
        if (!passwordValid)
        {
            await userManager.AccessFailedAsync(user);
            return null;
        }

        await userManager.ResetAccessFailedCountAsync(user);
        return user;
    }
}
