using FluentValidation;

namespace CRM.Medical.Application.Features.TestRequests.Queries.ListTestRequests;

public sealed class ListTestRequestsQueryValidator : AbstractValidator<ListTestRequestsQuery>
{
    public ListTestRequestsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
