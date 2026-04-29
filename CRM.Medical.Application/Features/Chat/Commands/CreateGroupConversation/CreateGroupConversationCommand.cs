using CRM.Medical.Application.Features.Chat.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Chat.Commands.CreateGroupConversation;

public sealed record CreateGroupConversationCommand(
    string ActorUserId,
    string Title,
    IReadOnlyList<string> ParticipantUserIds)
    : IRequest<ConversationSummaryDto>;
