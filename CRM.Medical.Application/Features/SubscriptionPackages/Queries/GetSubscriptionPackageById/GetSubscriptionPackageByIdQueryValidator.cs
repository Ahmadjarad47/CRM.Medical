using FluentValidation;

namespace CRM.Medical.Application.Features.SubscriptionPackages.Queries.GetSubscriptionPackageById;

public sealed class GetSubscriptionPackageByIdQueryValidator : AbstractValidator<GetSubscriptionPackageByIdQuery>
{
    public GetSubscriptionPackageByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);
    }
}
