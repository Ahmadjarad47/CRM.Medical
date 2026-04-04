using FluentValidation;

namespace CRM.Medical.Application.Features.Appointments.Commands.DoctorCreateAppointment;

public sealed class DoctorCreateAppointmentCommandValidator : AbstractValidator<DoctorCreateAppointmentCommand>
{
    public DoctorCreateAppointmentCommandValidator(AppointmentFormFieldsValidator fieldsValidator)
    {
        RuleFor(x => x.DoctorUserId).NotEmpty();
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.LabPartnerId).NotEmpty();
        RuleFor(x => x.Fields).SetValidator(fieldsValidator);
    }
}
