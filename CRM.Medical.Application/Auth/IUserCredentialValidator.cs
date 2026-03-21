namespace CRM.Medical.Application.Auth;

public interface IUserCredentialValidator
{
    Task<CredentialValidationResult> ValidateAsync(
        string email,
        string password,
        CancellationToken cancellationToken);
}

public sealed record CredentialValidationResult(AuthenticatedUser? User, CredentialFailureReason? FailureReason);

public enum CredentialFailureReason
{
    UserNotFound = 1,
    InvalidPassword = 2,
    LockedOut = 3,
    EmailNotConfirmed = 4,
}
