using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Auth;

public interface IUserCredentialValidator
{
    Task<User?> ValidateAsync(string email, string password, CancellationToken ct = default);
}
