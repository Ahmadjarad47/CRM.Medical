using CRM.Medical.Application.Auth;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace CRM.Medical.Application.Features.Auth.RefreshToken;

public sealed class RefreshTokenCommandHandler(
    IRefreshTokenService refreshTokenService,
    IJwtTokenGenerator tokenGenerator)
    : IRequestHandler<RefreshTokenCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var rotateResult = await refreshTokenService.RotateAsync(request.UserId, request.RefreshToken, cancellationToken);
        if (rotateResult is null)
        {
            throw new ValidationException(
                [new ValidationFailure(nameof(RefreshTokenCommand.RefreshToken), "Refresh token is invalid or expired.")]);
        }

        var (accessToken, accessTokenExpiresAtUtc) = tokenGenerator.CreateAccessToken(rotateResult.User);
        return new LoginResponse(
            rotateResult.User.Id,
            accessToken,
            accessTokenExpiresAtUtc,
            rotateResult.Token,
            rotateResult.ExpiresAtUtc,
            rotateResult.User.Email,
            rotateResult.User.DisplayName);
    }
}
