using FluentValidation;

namespace CRM.Medical.Application.Features.Appointments;

public sealed class AppointmentFormFieldsValidator : AbstractValidator<AppointmentFormFields>
{
    public AppointmentFormFieldsValidator()
    {
        RuleFor(x => x.AppointmentTypeId)
            .GreaterThan(0);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(300);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(x => x.Notes)
            .MaximumLength(4000);

        RuleFor(x => x.LocationType)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Address)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90)
            .When(x => x.Latitude.HasValue);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180)
            .When(x => x.Longitude.HasValue);
    }
}
