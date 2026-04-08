using FluentValidation;

namespace CRM.Medical.Application.Features.MedicalTests.Queries.ListMedicalTests;

public sealed class ListMedicalTestsQueryValidator : AbstractValidator<ListMedicalTestsQuery>
{
    public ListMedicalTestsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
