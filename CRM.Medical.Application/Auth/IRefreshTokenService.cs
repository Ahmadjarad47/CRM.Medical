namespace CRM.Medical.Application.Auth;

public interface IRefreshTokenService
{
    Task<RefreshTokenIssueResult> IssueAsync(string userId, CancellationToken cancellationToken);

    Task<RefreshTokenIssueResult?> RotateAsync(
        string userId,
        string refreshToken,
        CancellationToken cancellationToken);
}

public sealed record RefreshTokenIssueResult(
    string Token,
    DateTimeOffset ExpiresAtUtc,
    AuthenticatedUser User);
