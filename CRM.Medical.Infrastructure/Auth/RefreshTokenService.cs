using System.Security.Cryptography;
using CRM.Medical.Application.Auth;
using CRM.Medical.Application.Common.Time;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CRM.Medical.Infrastructure.Auth;

public sealed class RefreshTokenService(
    MedicalDbContext dbContext,
    IOptions<JwtSettings> jwtOptions,
    IDateTimeProvider dateTimeProvider)
    : IRefreshTokenService
{
    private readonly JwtSettings _settings = jwtOptions.Value;

    public async Task<string> GenerateAsync(string userId, CancellationToken ct = default)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var now = dateTimeProvider.UtcNow;

        dbContext.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = token,
            UserId = userId,
            ExpiresAt = now.AddDays(_settings.RefreshTokenExpirationDays),
            CreatedAt = now,
            IsRevoked = false
        });

        await dbContext.SaveChangesAsync(ct);
        return token;
    }

    public async Task<string?> ValidateAndGetUserIdAsync(string token, CancellationToken ct = default)
    {
        var refreshToken = await dbContext.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Token == token, ct);

        if (refreshToken is null || refreshToken.IsRevoked)
            return null;

        if (refreshToken.ExpiresAt < dateTimeProvider.UtcNow)
            return null;

        return refreshToken.UserId;
    }

    public async Task RevokeAsync(string token, CancellationToken ct = default)
    {
        var refreshToken = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == token, ct);

        if (refreshToken is null)
            return;

        refreshToken.IsRevoked = true;
        refreshToken.RevokedAt = dateTimeProvider.UtcNow;
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task RevokeAllForUserAsync(string userId, CancellationToken ct = default)
    {
        var now = dateTimeProvider.UtcNow;
        await dbContext.RefreshTokens
            .Where(t => t.UserId == userId && !t.IsRevoked)
            .ExecuteUpdateAsync(s => s
                .SetProperty(t => t.IsRevoked, true)
                .SetProperty(t => t.RevokedAt, now), ct);
    }
}
