using FluentValidation;

namespace CRM.Medical.Application.Features.Chat.Commands.LeaveConversation;

public sealed class LeaveConversationCommandValidator : AbstractValidator<LeaveConversationCommand>
{
    public LeaveConversationCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ConversationId).NotEmpty();
    }
}
