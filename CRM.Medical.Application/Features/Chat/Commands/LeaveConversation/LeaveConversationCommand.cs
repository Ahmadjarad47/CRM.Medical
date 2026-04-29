using MediatR;

namespace CRM.Medical.Application.Features.Chat.Commands.LeaveConversation;

public sealed record LeaveConversationCommand(string UserId, Guid ConversationId) : IRequest<Unit>;
