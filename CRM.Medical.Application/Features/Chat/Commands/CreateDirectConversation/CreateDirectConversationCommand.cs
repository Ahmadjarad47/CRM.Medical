using CRM.Medical.Application.Features.Chat.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Chat.Commands.CreateDirectConversation;

public sealed record CreateDirectConversationCommand(string ActorUserId, string OtherUserId)
    : IRequest<ConversationSummaryDto>;
