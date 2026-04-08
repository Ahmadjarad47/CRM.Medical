using CRM.Medical.Domain.Constants;
using FluentValidation;

namespace CRM.Medical.Application.Features.MedicalTests.Commands.CreateMedicalTest;

public sealed class CreateMedicalTestCommandValidator : AbstractValidator<CreateMedicalTestCommand>
{
    public CreateMedicalTestCommandValidator()
    {
        RuleFor(x => x.NameAr).NotEmpty().MaximumLength(500);
        RuleFor(x => x.NameEn).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(200);
        RuleFor(x => x.SampleType).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Status)
            .NotEmpty()
            .MaximumLength(64)
            .Must(s => MedicalTestStatuses.All.Contains(s))
            .WithMessage("Status must be a known medical test lifecycle value.");
        RuleFor(x => x.CreatedByUserId).NotEmpty();
    }
}
