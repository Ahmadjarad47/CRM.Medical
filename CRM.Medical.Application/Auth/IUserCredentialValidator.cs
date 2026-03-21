namespace CRM.Medical.Application.Auth;

public interface IUserCredentialValidator
{
    Task<AuthenticatedUser?> ValidateAsync(string email, string password, CancellationToken cancellationToken);
}
