using FluentValidation;

namespace CRM.Medical.Application.Features.Chat.Commands.CreateGroupConversation;

public sealed class CreateGroupConversationCommandValidator : AbstractValidator<CreateGroupConversationCommand>
{
    public CreateGroupConversationCommandValidator()
    {
        RuleFor(x => x.ActorUserId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(500);
        RuleFor(x => x.ParticipantUserIds).NotNull();
    }
}
