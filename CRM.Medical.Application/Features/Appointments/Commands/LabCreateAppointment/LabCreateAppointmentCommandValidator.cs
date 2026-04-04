using FluentValidation;

namespace CRM.Medical.Application.Features.Appointments.Commands.LabCreateAppointment;

public sealed class LabCreateAppointmentCommandValidator : AbstractValidator<LabCreateAppointmentCommand>
{
    public LabCreateAppointmentCommandValidator(AppointmentFormFieldsValidator fieldsValidator)
    {
        RuleFor(x => x.LabUserId).NotEmpty();
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.DoctorId).NotEmpty();
        RuleFor(x => x.Fields).SetValidator(fieldsValidator);
    }
}
