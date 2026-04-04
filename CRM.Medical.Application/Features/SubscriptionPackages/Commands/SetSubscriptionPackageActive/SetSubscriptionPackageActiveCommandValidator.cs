using FluentValidation;

namespace CRM.Medical.Application.Features.SubscriptionPackages.Commands.SetSubscriptionPackageActive;

public sealed class SetSubscriptionPackageActiveCommandValidator : AbstractValidator<SetSubscriptionPackageActiveCommand>
{
    public SetSubscriptionPackageActiveCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);
    }
}
