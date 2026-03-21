using CRM.Medical.Application.Auth;
using MediatR;

namespace CRM.Medical.Application.Features.Auth.Login;

public sealed class LoginCommandHandler(IUserCredentialValidator credentialValidator, IJwtTokenGenerator tokenGenerator)
    : IRequestHandler<LoginCommand, LoginResponse?>
{
    public async Task<LoginResponse?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();
        var user = await credentialValidator.ValidateAsync(email, request.Password, cancellationToken);
        if (user is null)
            return null;

        var (accessToken, expiresAtUtc) = tokenGenerator.CreateAccessToken(user);
        return new LoginResponse(accessToken, expiresAtUtc, user.Email, user.DisplayName);
    }
}
