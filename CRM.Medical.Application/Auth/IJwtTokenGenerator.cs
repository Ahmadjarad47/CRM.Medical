namespace CRM.Medical.Application.Auth;

public interface IJwtTokenGenerator
{
    (string Token, DateTimeOffset ExpiresAtUtc) CreateAccessToken(AuthenticatedUser user);
}
