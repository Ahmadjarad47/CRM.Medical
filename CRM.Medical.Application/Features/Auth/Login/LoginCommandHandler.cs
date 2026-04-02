using CRM.Medical.Application.Auth;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Auth.Login;

public sealed class LoginCommandHandler(
    IUserCredentialValidator credentialValidator,
    UserManager<User> userManager,
    IJwtTokenGenerator jwtTokenGenerator,
    IRefreshTokenService refreshTokenService)
    : IRequestHandler<LoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await credentialValidator.ValidateAsync(request.Email, request.Password, cancellationToken)
            ?? throw new ApplicationBadRequestException("Invalid email or password.");

        if (!user.IsActive)
            throw new ApplicationForbiddenException("Account is deactivated. Please contact an administrator.");

        var roles = await userManager.GetRolesAsync(user);

        var allClaims = await userManager.GetClaimsAsync(user);
        var permissions = allClaims
            .Where(c => c.Type == UserPermissions.ClaimType)
            .Select(c => c.Value)
            .ToList()
            .AsReadOnly();

        var authenticatedUser = new AuthenticatedUser(
            user.Id,
            user.Email!,
            user.FullName,
            roles.ToList().AsReadOnly(),
            permissions);

        var accessToken = jwtTokenGenerator.GenerateToken(authenticatedUser);
        var refreshToken = await refreshTokenService.GenerateAsync(user.Id, cancellationToken);
        var expiresAt = jwtTokenGenerator.GetExpiration();

        return new LoginResponse(accessToken, refreshToken, expiresAt, authenticatedUser);
    }
}
