using FluentValidation;

namespace CRM.Medical.Application.Features.SubscriptionPackages.Queries.ListSubscriptionPackages;

public sealed class ListSubscriptionPackagesQueryValidator : AbstractValidator<ListSubscriptionPackagesQuery>
{
    public ListSubscriptionPackagesQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}
