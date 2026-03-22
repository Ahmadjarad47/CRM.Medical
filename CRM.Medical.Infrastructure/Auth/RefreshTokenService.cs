using System.Security.Cryptography;
using System.Text;
using CRM.Medical.Application.Auth;
using CRM.Medical.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace CRM.Medical.Infrastructure.Auth;

public sealed class RefreshTokenService(
    UserManager<User> userManager,
    IOptions<JwtSettings> jwtOptions)
    : IRefreshTokenService
{
    private const string LoginProvider = "CRM.Medical.Auth";
    private const string RefreshTokenHashName = "RefreshTokenHash";
    private const string RefreshTokenExpiryName = "RefreshTokenExpiryUnix";

    private readonly JwtSettings _jwt = jwtOptions.Value;

    public async Task<RefreshTokenIssueResult> IssueAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new KeyNotFoundException($"User '{userId}' was not found.");
        return await IssueForUserAsync(user);
    }

    public async Task<RefreshTokenIssueResult?> RotateAsync(
        string userId,
        string refreshToken,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return null;

        var storedHash = await userManager.GetAuthenticationTokenAsync(
            user,
            LoginProvider,
            RefreshTokenHashName);
        var storedExpiryUnix = await userManager.GetAuthenticationTokenAsync(
            user,
            LoginProvider,
            RefreshTokenExpiryName);
        if (string.IsNullOrWhiteSpace(storedHash) || string.IsNullOrWhiteSpace(storedExpiryUnix))
            return null;

        if (!long.TryParse(storedExpiryUnix, out var expiryUnix))
            return null;

        var expiresAtUtc = DateTimeOffset.FromUnixTimeSeconds(expiryUnix);
        if (expiresAtUtc <= DateTimeOffset.UtcNow)
            return null;

        var presentedHash = ComputeHash(refreshToken);
        if (!FixedTimeEquals(storedHash, presentedHash))
            return null;

        return await IssueForUserAsync(user);
    }

    private async Task<RefreshTokenIssueResult> IssueForUserAsync(User user)
    {
        var refreshToken = GenerateRefreshToken();
        var refreshTokenHash = ComputeHash(refreshToken);
        var refreshTokenExpiresAtUtc = DateTimeOffset.UtcNow.AddDays(
            _jwt.RefreshTokenExpirationDays);

        await userManager.SetAuthenticationTokenAsync(
            user,
            LoginProvider,
            RefreshTokenHashName,
            refreshTokenHash);
        await userManager.SetAuthenticationTokenAsync(
            user,
            LoginProvider,
            RefreshTokenExpiryName,
            refreshTokenExpiresAtUtc.ToUnixTimeSeconds().ToString());

        return new RefreshTokenIssueResult(
            refreshToken,
            refreshTokenExpiresAtUtc,
            new AuthenticatedUser(
                user.Id,
                user.Email ?? string.Empty,
                user.DisplayName));
    }

    private static string GenerateRefreshToken()
    {
        Span<byte> bytes = stackalloc byte[48];
        RandomNumberGenerator.Fill(bytes);
        return WebEncoders.Base64UrlEncode(bytes);
    }

    private static string ComputeHash(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }

    private static bool FixedTimeEquals(string leftHex, string rightHex)
    {
        var leftBytes = Convert.FromHexString(leftHex);
        var rightBytes = Convert.FromHexString(rightHex);
        return CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
    }
}
