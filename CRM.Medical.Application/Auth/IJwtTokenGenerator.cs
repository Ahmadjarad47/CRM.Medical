namespace CRM.Medical.Application.Auth;

public interface IJwtTokenGenerator
{
    string GenerateToken(AuthenticatedUser user);
    DateTime GetExpiration();
}
