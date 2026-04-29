using FluentValidation;

namespace CRM.Medical.Application.Features.Chat.Commands.SendMessage;

public sealed class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
        RuleFor(x => x.SenderUserId).NotEmpty();
        RuleFor(x => x.ConversationId).NotEmpty();
        RuleFor(x => x.MessageType).IsInEnum();
    }
}
