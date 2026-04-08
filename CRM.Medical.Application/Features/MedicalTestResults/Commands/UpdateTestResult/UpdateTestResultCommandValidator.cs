using CRM.Medical.Domain.Constants;
using FluentValidation;

namespace CRM.Medical.Application.Features.MedicalTestResults.Commands.UpdateTestResult;

public sealed class UpdateTestResultCommandValidator : AbstractValidator<UpdateTestResultCommand>
{
    public UpdateTestResultCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Status)
            .NotEmpty()
            .MaximumLength(64)
            .Must(s => TestResultStatuses.All.Contains(s))
            .WithMessage("Status must be a known test result lifecycle value.");
        RuleFor(x => x.PdfUrl).MaximumLength(2048);
    }
}
