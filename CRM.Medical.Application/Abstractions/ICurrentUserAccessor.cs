namespace CRM.Medical.Application.Abstractions;

/// <summary>
/// Provides the authenticated user id for the current request (when available).
/// </summary>
public interface ICurrentUserAccessor
{
    /// <summary>Identity user id (<c>sub</c> / NameIdentifier), or null if anonymous.</summary>
    string? UserId { get; }

    /// <summary>Role claims from the access token.</summary>
    IReadOnlyList<string> Roles { get; }

    /// <summary>Whether the user carries the named permission claim.</summary>
    bool HasPermission(string permissionName);

    /// <summary>Case-insensitive role check against the token.</summary>
    bool IsInRole(string roleName);
}
