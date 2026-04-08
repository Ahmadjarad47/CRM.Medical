using FluentValidation;

namespace CRM.Medical.Application.Features.MedicalTestResults.Commands.DeleteTestResult;

public sealed class DeleteTestResultCommandValidator : AbstractValidator<DeleteTestResultCommand>
{
    public DeleteTestResultCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
