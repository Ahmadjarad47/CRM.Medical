namespace CRM.Medical.Application.Features.Chat.Services;

/// <summary>
/// Validates who may start conversations and with whom (role + healthcare workflow links).
/// </summary>
public interface IChatAuthorizationService
{
    Task EnsureCanChatWithPeersAsync(
        string actorUserId,
        IReadOnlyCollection<string> otherUserIds,
        CancellationToken cancellationToken = default);

    Task EnsureActiveParticipantAsync(
        string userId,
        Guid conversationId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetPeerUserIdsActorMayChatAsync(
        string actorUserId,
        CancellationToken cancellationToken = default);

    /// <summary>Restricts an arbitrary user id list to peers the actor may chat with.</summary>
    Task<IReadOnlyList<string>> FilterToPeersActorMayChatAsync(
        string actorUserId,
        IReadOnlyCollection<string> candidateUserIds,
        CancellationToken cancellationToken = default);
}
