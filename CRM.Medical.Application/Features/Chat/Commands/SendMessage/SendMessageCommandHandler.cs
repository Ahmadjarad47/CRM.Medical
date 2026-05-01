using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Chat.DTOs;
using CRM.Medical.Application.Features.Chat.Mapping;
using CRM.Medical.Application.Features.Chat.Models;
using CRM.Medical.Application.Features.Chat.Persistence;
using CRM.Medical.Application.Features.Chat.Services;
using CRM.Medical.Domain.Chat;
using CRM.Medical.Domain.Entities;
using MediatR;

namespace CRM.Medical.Application.Features.Chat.Commands.SendMessage;

public sealed class SendMessageCommandHandler(
    IChatPersistence chatPersistence,
    IChatAuthorizationService chatAuthorization,
    IChatRealtimeNotifier realtimeNotifier,
    IChatUserSummaryLookup summaryLookup,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<SendMessageCommand, MessageDto>
{
    public async Task<MessageDto> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        await chatAuthorization.EnsureActiveParticipantAsync(request.SenderUserId, request.ConversationId, cancellationToken);

        if (request.MessageType == ChatMessageType.Text &&
            string.IsNullOrWhiteSpace(request.Text))
            throw new ApplicationBadRequestException("Message text is required.");

        if (request.MessageType is ChatMessageType.File or ChatMessageType.Image && string.IsNullOrWhiteSpace(request.FileUrl))
            throw new ApplicationBadRequestException("A file URL is required for file or image messages.");

        if (request.ReplyToId is { } replyId)
        {
            var replied = await chatPersistence.GetMessageAsync(replyId, cancellationToken)
                ?? throw new ApplicationNotFoundException("Reply target message was not found.");

            if (replied.ConversationId != request.ConversationId)
                throw new ApplicationBadRequestException("Cannot reply to a message outside this conversation.");
        }

        var now = dateTimeProvider.UtcNow;

        var entity = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = request.ConversationId,
            SenderId = request.SenderUserId,
            Text = request.Text?.Trim(),
            MessageType = request.MessageType,
            FileUrl = request.FileUrl,
            ReplyToId = request.ReplyToId,
            IsRead = false,
            CreatedAt = now,
            Attachments = []
        };

        await chatPersistence.AddMessageAsync(entity, cancellationToken);
        await chatPersistence.SaveChangesAsync(cancellationToken);

        var summaries = await summaryLookup.GetSummariesAsync([request.SenderUserId], cancellationToken);
        var senderSummary = summaries[request.SenderUserId];
        var dto = entity.ToDto(senderSummary);

        var payload = new ChatMessageRealtimePayload(
            dto.Id,
            dto.ConversationId,
            dto.SenderId,
            dto.SenderFullName,
            senderSummary,
            dto.Text,
            dto.MessageType,
            dto.FileUrl,
            dto.ReplyToId,
            dto.CreatedAtUtc);

        await realtimeNotifier.BroadcastNewMessageAsync(request.ConversationId, payload, cancellationToken);

        var participants = await chatPersistence.GetActiveParticipantsAsync(request.ConversationId, cancellationToken);
        foreach (var peer in participants.Where(p => p.UserId != request.SenderUserId))
        {
            await realtimeNotifier.NotifyConversationUpdatedAsync(
                peer.UserId,
                new ConversationUpdatedPayload(request.ConversationId, dto.Id),
                cancellationToken);
        }

        return dto;
    }
}
