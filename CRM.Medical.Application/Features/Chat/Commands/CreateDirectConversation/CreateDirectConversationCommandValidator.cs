using FluentValidation;

namespace CRM.Medical.Application.Features.Chat.Commands.CreateDirectConversation;

public sealed class CreateDirectConversationCommandValidator : AbstractValidator<CreateDirectConversationCommand>
{
    public CreateDirectConversationCommandValidator()
    {
        RuleFor(x => x.ActorUserId).NotEmpty();
        RuleFor(x => x.OtherUserId).NotEmpty();
    }
}
