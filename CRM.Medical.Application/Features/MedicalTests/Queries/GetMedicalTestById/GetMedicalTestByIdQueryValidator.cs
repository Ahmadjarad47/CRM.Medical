using FluentValidation;

namespace CRM.Medical.Application.Features.MedicalTests.Queries.GetMedicalTestById;

public sealed class GetMedicalTestByIdQueryValidator : AbstractValidator<GetMedicalTestByIdQuery>
{
    public GetMedicalTestByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
