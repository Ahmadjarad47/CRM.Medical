using CRM.Medical.Application.Auth;
using CRM.Medical.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Infrastructure.Auth;

public sealed class UserCredentialValidator(
    UserManager<User> userManager,
    SignInManager<User> signInManager) : IUserCredentialValidator
{
    public async Task<CredentialValidationResult> ValidateAsync(
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return new CredentialValidationResult(null, CredentialFailureReason.UserNotFound);

        var signInResult = await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        if (signInResult.IsLockedOut)
            return new CredentialValidationResult(null, CredentialFailureReason.LockedOut);

        if (signInResult.IsNotAllowed)
            return new CredentialValidationResult(null, CredentialFailureReason.EmailNotConfirmed);

        if (!signInResult.Succeeded)
            return new CredentialValidationResult(null, CredentialFailureReason.InvalidPassword);

        var resolvedEmail = user.Email ?? email;
        return new CredentialValidationResult(
            new AuthenticatedUser(user.Id, resolvedEmail, user.DisplayName),
            null);
    }
}
