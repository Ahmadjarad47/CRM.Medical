using FluentValidation;

namespace CRM.Medical.Application.Features.Appointments.Commands.ConfirmAppointment;

public sealed class ConfirmAppointmentCommandValidator : AbstractValidator<ConfirmAppointmentCommand>
{
    public ConfirmAppointmentCommandValidator()
    {
        RuleFor(x => x.AppointmentId).GreaterThan(0);
        RuleFor(x => x.ActingUserId).NotEmpty();
    }
}
