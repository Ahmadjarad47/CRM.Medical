using FluentValidation;

namespace CRM.Medical.Application.Features.AppointmentTypes.Commands.UpdateAppointmentType;

public sealed class UpdateAppointmentTypeCommandValidator : AbstractValidator<UpdateAppointmentTypeCommand>
{
    public UpdateAppointmentTypeCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}
