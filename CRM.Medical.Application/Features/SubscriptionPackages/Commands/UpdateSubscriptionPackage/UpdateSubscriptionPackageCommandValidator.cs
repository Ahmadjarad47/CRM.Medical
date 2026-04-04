using FluentValidation;

namespace CRM.Medical.Application.Features.SubscriptionPackages.Commands.UpdateSubscriptionPackage;

public sealed class UpdateSubscriptionPackageCommandValidator : AbstractValidator<UpdateSubscriptionPackageCommand>
{
    public UpdateSubscriptionPackageCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

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
