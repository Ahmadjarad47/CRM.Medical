using FluentValidation;

namespace CRM.Medical.Application.Features.Chat.Commands.MarkMessageAsRead;

public sealed class MarkMessageAsReadCommandValidator : AbstractValidator<MarkMessageAsReadCommand>
{
    public MarkMessageAsReadCommandValidator()
    {
        RuleFor(x => x.ReaderUserId).NotEmpty();
        RuleFor(x => x.MessageId).NotEmpty();
    }
}
