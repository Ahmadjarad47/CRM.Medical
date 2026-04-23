using CRM.Medical.Application.Configuration.S3;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace CRM.Medical.Application.Features.Complaints.Commands.SubmitComplaint;

public sealed class SubmitComplaintCommandValidator : AbstractValidator<SubmitComplaintCommand>
{
    public SubmitComplaintCommandValidator(IOptions<S3StorageSettings> s3Options)
    {
        var maxBytes = s3Options.Value.MaxAttachmentBytes;

        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(8000);

        RuleFor(x => x.Attachment)
            .Must(f => f == null || f.Length <= maxBytes)
            .WithMessage($"Attachment must not exceed {maxBytes} bytes.");
    }
}
