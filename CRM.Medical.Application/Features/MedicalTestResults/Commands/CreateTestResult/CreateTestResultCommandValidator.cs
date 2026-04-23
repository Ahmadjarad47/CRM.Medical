using CRM.Medical.Application.Configuration.S3;
using CRM.Medical.Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace CRM.Medical.Application.Features.MedicalTestResults.Commands.CreateTestResult;

public sealed class CreateTestResultCommandValidator : AbstractValidator<CreateTestResultCommand>
{
    public CreateTestResultCommandValidator(IOptions<S3StorageSettings> s3Options)
    {
        var maxBytes = s3Options.Value.MaxAttachmentBytes;

        RuleFor(x => x.TestRequestId).GreaterThan(0);
        RuleFor(x => x.Status)
            .NotEmpty()
            .MaximumLength(64)
            .Must(s => TestResultStatuses.All.Contains(s))
            .WithMessage("Status must be a known test result lifecycle value.");
        RuleFor(x => x.CreatedByUserId).NotEmpty();

        RuleFor(x => x.PdfFile)
            .Must(f => f == null || f.Length <= maxBytes)
            .WithMessage($"PDF must not exceed {maxBytes} bytes.");
    }
}
