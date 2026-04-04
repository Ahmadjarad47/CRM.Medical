using FluentValidation;

namespace CRM.Medical.Application.Features.Appointments.Commands.PatientBookAppointment;

public sealed class PatientBookAppointmentCommandValidator : AbstractValidator<PatientBookAppointmentCommand>
{
    public PatientBookAppointmentCommandValidator(AppointmentFormFieldsValidator fieldsValidator)
    {
        RuleFor(x => x.PatientUserId).NotEmpty();
        RuleFor(x => x.Fields).SetValidator(fieldsValidator);

        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.DoctorId) || !string.IsNullOrWhiteSpace(x.LabPartnerId))
            .WithMessage("Either a doctor or a lab partner must be selected.");
    }
}
