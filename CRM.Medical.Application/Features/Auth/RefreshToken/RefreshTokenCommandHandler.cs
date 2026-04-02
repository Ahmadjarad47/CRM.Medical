using CRM.Medical.Application.Auth;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Auth.RefreshToken;

public sealed class RefreshTokenCommandHandler(
    IRefreshTokenService refreshTokenService,
    UserManager<User> userManager,
    IJwtTokenGenerator jwtTokenGenerator)
    : IRequestHandler<RefreshTokenCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var userId = await refreshTokenService.ValidateAndGetUserIdAsync(request.Token, cancellationToken)
            ?? throw new ApplicationBadRequestException("Invalid or expired refresh token.");

        var user = await userManager.FindByIdAsync(userId)
            ?? throw new ApplicationNotFoundException("User not found.");

        if (!user.IsActive)
            throw new ApplicationForbiddenException("Account is deactivated.");

        await refreshTokenService.RevokeAsync(request.Token, cancellationToken);

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
        var newRefreshToken = await refreshTokenService.GenerateAsync(user.Id, cancellationToken);
        var expiresAt = jwtTokenGenerator.GetExpiration();

        return new LoginResponse(accessToken, newRefreshToken, expiresAt, authenticatedUser);
    }
}
