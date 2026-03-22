namespace CRM.Medical.Application.Auth;

public sealed record LoginResponse(
    string UserId,
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAtUtc,
    string Email,
    string DisplayName);
