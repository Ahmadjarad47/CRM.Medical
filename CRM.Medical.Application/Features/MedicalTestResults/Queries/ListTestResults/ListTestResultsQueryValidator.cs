using FluentValidation;

namespace CRM.Medical.Application.Features.MedicalTestResults.Queries.ListTestResults;

public sealed class ListTestResultsQueryValidator : AbstractValidator<ListTestResultsQuery>
{
    public ListTestResultsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
