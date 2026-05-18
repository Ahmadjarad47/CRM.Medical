namespace CRM.Medical.Application.Abstractions;

/// <summary>
/// Provides the authenticated user id for the current request (when available).
/// </summary>
public interface ICurrentUserAccessor
{
    /// <summary>Identity user id (<c>sub</c> / NameIdentifier), or null if anonymous.</summary>
    string? UserId { get; }

    /// <summary>Email claim from the access token, or null if unavailable.</summary>
    string? Email { get; }

    /// <summary>Role claims from the access token.</summary>
    IReadOnlyList<string> Roles { get; }

    /// <summary>Tenant claim from the access token, or null if unavailable.</summary>
    string? TenantId { get; }

    /// <summary>Case-insensitive role check against the token.</summary>
    bool IsInRole(string roleName);
}
