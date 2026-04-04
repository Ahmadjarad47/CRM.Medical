using FluentValidation;

namespace CRM.Medical.Application.Features.AppointmentTypes.Commands.CreateAppointmentType;

public sealed class CreateAppointmentTypeCommandValidator : AbstractValidator<CreateAppointmentTypeCommand>
{
    public CreateAppointmentTypeCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}
