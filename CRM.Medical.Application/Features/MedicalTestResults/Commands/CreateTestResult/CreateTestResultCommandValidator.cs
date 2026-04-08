using CRM.Medical.Domain.Constants;
using FluentValidation;

namespace CRM.Medical.Application.Features.MedicalTestResults.Commands.CreateTestResult;

public sealed class CreateTestResultCommandValidator : AbstractValidator<CreateTestResultCommand>
{
    public CreateTestResultCommandValidator()
    {
        RuleFor(x => x.TestRequestId).GreaterThan(0);
        RuleFor(x => x.Status)
            .NotEmpty()
            .MaximumLength(64)
            .Must(s => TestResultStatuses.All.Contains(s))
            .WithMessage("Status must be a known test result lifecycle value.");
        RuleFor(x => x.PdfUrl).MaximumLength(2048);
        RuleFor(x => x.CreatedByUserId).NotEmpty();
    }
}
