using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Chat.Services;
using CRM.Medical.Application.Authorization;
using CRM.Medical.Domain.Entities;
using CRM.Medical.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Chat;

/// <summary>
/// Enforces healthcare chat routing rules using Identity roles and <see cref="TestRequest"/> workflow links.
/// </summary>
public sealed class ChatAuthorizationService(
    MedicalDbContext db,
    UserManager<User> userManager,
    IAccessPolicyEvaluator accessPolicyEvaluator)
    : IChatAuthorizationService
{
    private readonly MedicalDbContext _db = db;
    private readonly UserManager<User> _userManager = userManager;

    public async Task EnsureActiveParticipantAsync(string userId, Guid conversationId, CancellationToken cancellationToken = default)
    {
        var scoped = await accessPolicyEvaluator.ApplyAsync(
            _db.ConversationParticipants.AsNoTracking(),
            "conversation_participants",
            "read",
            cancellationToken);
        var ok = await scoped.AnyAsync(p => p.ConversationId == conversationId && p.UserId == userId && p.LeftAt == null, cancellationToken);

        if (!ok)
            throw new ApplicationForbiddenException("You are not an active participant in this conversation.");
    }

    public async Task EnsureCanChatWithPeersAsync(
        string actorUserId,
        IReadOnlyCollection<string> otherUserIds,
        CancellationToken cancellationToken = default)
    {
        var actor = await _userManager.FindByIdAsync(actorUserId)
            ?? throw new ApplicationUnauthorizedException("Unable to identify the current user.");

        var distinctPeers = otherUserIds
            .Where(id => !string.Equals(id, actorUserId, StringComparison.Ordinal))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        foreach (var peerId in distinctPeers)
        {
            var peer = await _userManager.FindByIdAsync(peerId)
                ?? throw new ApplicationBadRequestException($"User '{peerId}' was not found.");

            if (!peer.IsActive)
                throw new ApplicationBadRequestException($"User '{peerId}' is not active.");

            var draft = new ConversationParticipant
            {
                Id = Guid.NewGuid(),
                ConversationId = Guid.NewGuid(),
                UserId = peer.Id,
                JoinedAt = DateTime.UtcNow,
                CreatedByUserId = actor.Id
            };
            var allowed = await accessPolicyEvaluator.CanAccessAsync(draft, "conversation_participants", "create", cancellationToken);
            if (!allowed)
                throw new ApplicationForbiddenException(
                    "You are not allowed to start or join a conversation with one or more selected users.");
        }
    }

    public async Task<IReadOnlyList<string>> FilterToPeersActorMayChatAsync(
        string actorUserId,
        IReadOnlyCollection<string> candidateUserIds,
        CancellationToken cancellationToken = default)
    {
        var actor = await _userManager.FindByIdAsync(actorUserId)
            ?? throw new ApplicationUnauthorizedException("Unable to identify the current user.");

        var distinct = candidateUserIds
            .Where(id => !string.Equals(id, actorUserId, StringComparison.Ordinal))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        var allowed = new List<string>();
        foreach (var peerId in distinct)
        {
            var peer = await _userManager.FindByIdAsync(peerId);
            if (peer is null || !peer.IsActive)
                continue;

            var draft = new ConversationParticipant
            {
                Id = Guid.NewGuid(),
                ConversationId = Guid.NewGuid(),
                UserId = peer.Id,
                JoinedAt = DateTime.UtcNow,
                CreatedByUserId = actor.Id
            };
            if (await accessPolicyEvaluator.CanAccessAsync(draft, "conversation_participants", "create", cancellationToken))
                allowed.Add(peerId);
        }

        return allowed;
    }

    
}
