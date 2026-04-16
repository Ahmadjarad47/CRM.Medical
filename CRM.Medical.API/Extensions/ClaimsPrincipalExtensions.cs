using System.Security.Claims;
using CRM.Medical.Application.Exceptions;

namespace CRM.Medical.API.Extensions;

/// <summary>
/// Centralizes reading the authenticated user id (JWT <c>sub</c> / NameIdentifier) from <see cref="ClaimsPrincipal"/>.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    public static string GetRequiredUserId(this ClaimsPrincipal user)
    {
        var id = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return !string.IsNullOrEmpty(id)
            ? id
            : throw new ApplicationUnauthorizedException("Unable to identify the current user.");
    }

    public static string GetRequiredRole(this ClaimsPrincipal user)
    {
        var role = user.FindFirstValue(ClaimTypes.Role);
        return !string.IsNullOrEmpty(role)
            ? role
            : throw new ApplicationUnauthorizedException("Unable to identify the current user's role.");
    }
}
