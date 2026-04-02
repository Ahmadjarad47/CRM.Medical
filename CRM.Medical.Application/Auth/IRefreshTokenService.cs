namespace CRM.Medical.Application.Auth;

public interface IRefreshTokenService
{
    Task<string> GenerateAsync(string userId, CancellationToken ct = default);
    Task<string?> ValidateAndGetUserIdAsync(string token, CancellationToken ct = default);
    Task RevokeAsync(string token, CancellationToken ct = default);
    Task RevokeAllForUserAsync(string userId, CancellationToken ct = default);
}
