using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Chat.DTOs;
using CRM.Medical.Application.Features.Chat.Persistence;
using CRM.Medical.Application.Features.Chat.Services;
using CRM.Medical.Domain.Chat;
using CRM.Medical.Domain.Entities;
using MediatR;

namespace CRM.Medical.Application.Features.Chat.Commands.CreateGroupConversation;

public sealed class CreateGroupConversationCommandHandler(
    IChatPersistence chatPersistence,
    IChatAuthorizationService chatAuthorization,
    IChatUserSummaryLookup summaryLookup,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<CreateGroupConversationCommand, ConversationSummaryDto>
{
    public async Task<ConversationSummaryDto> Handle(CreateGroupConversationCommand request, CancellationToken cancellationToken)
    {
        var distinctPeers = request.ParticipantUserIds
            .Where(id => !string.Equals(id, request.ActorUserId, StringComparison.Ordinal))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        if (distinctPeers.Count == 0)
            throw new ApplicationBadRequestException("A group conversation requires at least one other participant.");

        await chatAuthorization.EnsureCanChatWithPeersAsync(request.ActorUserId, distinctPeers, cancellationToken);

        var now = dateTimeProvider.UtcNow;
        var conversationId = Guid.NewGuid();

        var conversation = new Conversation
        {
            Id = conversationId,
            Type = ConversationType.Group,
            Title = request.Title.Trim(),
            CreatedAt = now,
            CreatedByUserId = request.ActorUserId
        };

        var participants = new List<ConversationParticipant>
        {
            new()
            {
                Id = Guid.NewGuid(),
                ConversationId = conversationId,
                UserId = request.ActorUserId,
                JoinedAt = now,
                Role = ConversationParticipantRole.Owner,
                CreatedAt = now
            }
        };

        foreach (var uid in distinctPeers)
        {
            participants.Add(new ConversationParticipant
            {
                Id = Guid.NewGuid(),
                ConversationId = conversationId,
                UserId = uid,
                JoinedAt = now,
                Role = ConversationParticipantRole.Member,
                CreatedAt = now
            });
        }

        await chatPersistence.AddConversationAsync(conversation, cancellationToken);
        await chatPersistence.AddParticipantsAsync(participants, cancellationToken);
        await chatPersistence.SaveChangesAsync(cancellationToken);

        var unread = await chatPersistence.CountUnreadForUserAsync(request.ActorUserId, conversationId, cancellationToken);

        var summaries = await summaryLookup.GetSummariesAsync([request.ActorUserId], cancellationToken);
        summaries.TryGetValue(request.ActorUserId, out var createdByUser);

        return new ConversationSummaryDto(
            conversation.Id,
            conversation.Type,
            conversation.Title,
            conversation.CreatedAt,
            conversation.CreatedByUserId,
            createdByUser,
            null,
            unread);
    }
}
