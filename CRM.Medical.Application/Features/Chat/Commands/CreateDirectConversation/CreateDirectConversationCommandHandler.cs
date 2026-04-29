using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Chat.DTOs;
using CRM.Medical.Application.Features.Chat.Persistence;
using CRM.Medical.Application.Features.Chat.Services;
using CRM.Medical.Domain.Chat;
using CRM.Medical.Domain.Entities;
using MediatR;

namespace CRM.Medical.Application.Features.Chat.Commands.CreateDirectConversation;

public sealed class CreateDirectConversationCommandHandler(
    IChatPersistence chatPersistence,
    IChatAuthorizationService chatAuthorization,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<CreateDirectConversationCommand, ConversationSummaryDto>
{
    public async Task<ConversationSummaryDto> Handle(CreateDirectConversationCommand request, CancellationToken cancellationToken)
    {
        if (string.Equals(request.ActorUserId, request.OtherUserId, StringComparison.Ordinal))
            throw new ApplicationBadRequestException("Cannot create a conversation with yourself.");

        await chatAuthorization.EnsureCanChatWithPeersAsync(
            request.ActorUserId,
            [request.OtherUserId],
            cancellationToken);

        var existing = await chatPersistence.FindActiveDirectConversationBetweenAsync(
            request.ActorUserId,
            request.OtherUserId,
            cancellationToken);

        if (existing is not null)
            return await MapSummaryAsync(existing, request.ActorUserId, cancellationToken);

        var now = dateTimeProvider.UtcNow;
        var conversationId = Guid.NewGuid();

        var conversation = new Conversation
        {
            Id = conversationId,
            Type = ConversationType.Direct,
            Title = null,
            CreatedAt = now,
            CreatedByUserId = request.ActorUserId
        };

        var participants = new[]
        {
            new ConversationParticipant
            {
                Id = Guid.NewGuid(),
                ConversationId = conversationId,
                UserId = request.ActorUserId,
                JoinedAt = now,
                Role = ConversationParticipantRole.Owner,
                CreatedAt = now
            },
            new ConversationParticipant
            {
                Id = Guid.NewGuid(),
                ConversationId = conversationId,
                UserId = request.OtherUserId,
                JoinedAt = now,
                Role = ConversationParticipantRole.Member,
                CreatedAt = now
            }
        };

        await chatPersistence.AddConversationAsync(conversation, cancellationToken);
        await chatPersistence.AddParticipantsAsync(participants, cancellationToken);
        await chatPersistence.SaveChangesAsync(cancellationToken);

        conversation.Participants = participants.ToList();
        return await MapSummaryAsync(conversation, request.ActorUserId, cancellationToken);
    }

    private async Task<ConversationSummaryDto> MapSummaryAsync(
        Conversation conversation,
        string viewerUserId,
        CancellationToken cancellationToken)
    {
        var unread = await chatPersistence.CountUnreadForUserAsync(viewerUserId, conversation.Id, cancellationToken);
        MessagePreviewDto? preview = null;

        var lastDict = await chatPersistence.GetLastMessagesByConversationIdsAsync(
            [conversation.Id],
            cancellationToken);

        if (lastDict.TryGetValue(conversation.Id, out var last) && last is not null)
        {
            preview = new MessagePreviewDto(last.Text, last.MessageType, last.CreatedAt);
        }

        return new ConversationSummaryDto(
            conversation.Id,
            conversation.Type,
            conversation.Title,
            conversation.CreatedAt,
            preview,
            unread);
    }
}
