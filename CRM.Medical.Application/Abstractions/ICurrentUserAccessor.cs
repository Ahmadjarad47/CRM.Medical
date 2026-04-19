namespace CRM.Medical.Application.Abstractions;

/// <summary>
/// Provides the authenticated user id for the current request (when available).
/// </summary>
public interface ICurrentUserAccessor
{
    /// <summary>Identity user id (<c>sub</c> / NameIdentifier), or null if anonymous.</summary>
    string? UserId { get; }
}
