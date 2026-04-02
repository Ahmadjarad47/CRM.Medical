namespace CRM.Medical.Application.Auth;

public sealed record LoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    AuthenticatedUser User);
