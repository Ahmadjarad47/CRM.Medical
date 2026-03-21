namespace CRM.Medical.Application.Auth;

public sealed record LoginResponse(string AccessToken, DateTimeOffset ExpiresAtUtc, string Email, string DisplayName);
