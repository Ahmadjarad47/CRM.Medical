using FluentValidation;

namespace CRM.Medical.Application.Features.SubscriptionPackages.Commands.CreateSubscriptionPackage;

public sealed class CreateSubscriptionPackageCommandValidator : AbstractValidator<CreateSubscriptionPackageCommand>
{
    public CreateSubscriptionPackageCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.ValidityDays)
            .GreaterThan(0);

        RuleFor(x => x.TargetAudience)
            .IsInEnum();
    }
}
