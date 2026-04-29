using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Features.Chat.Persistence;
using CRM.Medical.Application.Features.Chat.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Chat.Commands.LeaveConversation;

public sealed class LeaveConversationCommandHandler(
    IChatPersistence chatPersistence,
    IChatAuthorizationService chatAuthorization,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<LeaveConversationCommand, Unit>
{
    public async Task<Unit> Handle(LeaveConversationCommand request, CancellationToken cancellationToken)
    {
        await chatAuthorization.EnsureActiveParticipantAsync(request.UserId, request.ConversationId, cancellationToken);

        await chatPersistence.LeaveConversationAsync(
            request.UserId,
            request.ConversationId,
            dateTimeProvider.UtcNow,
            cancellationToken);

        return Unit.Value;
    }
}
