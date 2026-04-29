using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Chat.Models;
using CRM.Medical.Application.Features.Chat.Persistence;
using CRM.Medical.Application.Features.Chat.Services;
using CRM.Medical.Domain.Entities;
using MediatR;

namespace CRM.Medical.Application.Features.Chat.Commands.MarkMessageAsRead;

public sealed class MarkMessageAsReadCommandHandler(
    IChatPersistence chatPersistence,
    IChatAuthorizationService chatAuthorization,
    IChatRealtimeNotifier realtimeNotifier,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<MarkMessageAsReadCommand, Unit>
{
    public async Task<Unit> Handle(MarkMessageAsReadCommand request, CancellationToken cancellationToken)
    {
        var message = await chatPersistence.GetTrackedMessageAsync(request.MessageId, cancellationToken)
            ?? throw new ApplicationNotFoundException("Message was not found.");

        await chatAuthorization.EnsureActiveParticipantAsync(request.ReaderUserId, message.ConversationId, cancellationToken);

        if (string.Equals(message.SenderId, request.ReaderUserId, StringComparison.Ordinal))
            return Unit.Value;

        if (await chatPersistence.HasUserReadMessageAsync(message.Id, request.ReaderUserId, cancellationToken))
            return Unit.Value;

        var now = dateTimeProvider.UtcNow;
        var read = new MessageRead
        {
            Id = Guid.NewGuid(),
            MessageId = message.Id,
            UserId = request.ReaderUserId,
            ReadAt = now,
            CreatedAt = now
        };

        await chatPersistence.AddMessageReadAsync(read, cancellationToken);

        message.IsRead = true;
        message.UpdatedAt = now;

        await chatPersistence.SaveChangesAsync(cancellationToken);

        await realtimeNotifier.BroadcastReadReceiptAsync(
            message.ConversationId,
            new ChatReadReceiptPayload(message.Id, request.ReaderUserId, now),
            cancellationToken);

        return Unit.Value;
    }
}
