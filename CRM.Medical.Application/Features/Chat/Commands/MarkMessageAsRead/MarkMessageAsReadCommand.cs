using MediatR;

namespace CRM.Medical.Application.Features.Chat.Commands.MarkMessageAsRead;

public sealed record MarkMessageAsReadCommand(string ReaderUserId, Guid MessageId) : IRequest<Unit>;
