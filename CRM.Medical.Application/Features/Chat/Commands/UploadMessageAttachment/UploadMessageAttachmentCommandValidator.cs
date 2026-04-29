using FluentValidation;

namespace CRM.Medical.Application.Features.Chat.Commands.UploadMessageAttachment;

public sealed class UploadMessageAttachmentCommandValidator : AbstractValidator<UploadMessageAttachmentCommand>
{
    public UploadMessageAttachmentCommandValidator()
    {
        RuleFor(x => x.ActorUserId).NotEmpty();
        RuleFor(x => x.MessageId).NotEmpty();
        RuleFor(x => x.File).NotNull();
        RuleFor(x => x.File.Length).GreaterThan(0).When(x => x.File != null);
    }
}
