using FluentValidation;

namespace CRM.Medical.Application.Features.MedicalTests.Commands.DeleteMedicalTest;

public sealed class DeleteMedicalTestCommandValidator : AbstractValidator<DeleteMedicalTestCommand>
{
    public DeleteMedicalTestCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
