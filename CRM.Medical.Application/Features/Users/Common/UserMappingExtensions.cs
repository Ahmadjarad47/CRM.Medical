using CRM.Medical.Application.Features.Users.DTOs;
using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Features.Users.Common;

internal static class UserMappingExtensions
{
    public static UserSummaryDto ToSummaryDto(this User user)
    {
        var lockoutEndUtc = user.LockoutEnd?.ToUniversalTime();
        var isLocked = lockoutEndUtc is not null && lockoutEndUtc > DateTimeOffset.UtcNow;

        return new UserSummaryDto(
            user.Id,
            user.Email ?? string.Empty,
            user.DisplayName,
            user.EmailConfirmed,
            user.LockoutEnabled,
            isLocked);
    }

    public static UserDetailDto ToDetailDto(this User user)
    {
        var lockoutEndUtc = user.LockoutEnd?.ToUniversalTime();
        var isLocked = lockoutEndUtc is not null && lockoutEndUtc > DateTimeOffset.UtcNow;

        return new UserDetailDto(
            user.Id,
            user.Email ?? string.Empty,
            user.DisplayName,
            user.EmailConfirmed,
            user.LockoutEnabled,
            lockoutEndUtc,
            isLocked);
    }
}
