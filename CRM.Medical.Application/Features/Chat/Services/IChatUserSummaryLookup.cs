using CRM.Medical.Application.Features.Chat.DTOs;

namespace CRM.Medical.Application.Features.Chat.Services;

/// <summary>Merges <see cref="IChatUserProfileReader"/> with Redis-backed online status.</summary>
public interface IChatUserSummaryLookup
{
    Task<IReadOnlyDictionary<string, ChatUserSummaryDto>> GetSummariesAsync(
        IReadOnlyCollection<string> userIds,
        CancellationToken cancellationToken = default);
}
