namespace CRM.Medical.Application.Features.Chat.Services;

/// <summary>Loads identity profile fields for many users in one database round-trip (no presence / Redis).</summary>
public interface IChatUserProfileReader
{
    Task<IReadOnlyDictionary<string, ChatUserProfileSnapshot>> GetProfilesAsync(
        IReadOnlyCollection<string> userIds,
        CancellationToken cancellationToken = default);
}

/// <summary>Database-backed fields used to build chat user summaries.</summary>
public sealed record ChatUserProfileSnapshot(string FullName, string? Email, string? PhoneNumber, string? Role);
