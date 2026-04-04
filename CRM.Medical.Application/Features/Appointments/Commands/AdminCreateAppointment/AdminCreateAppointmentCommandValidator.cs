using FluentValidation;

namespace CRM.Medical.Application.Features.Appointments.Commands.AdminCreateAppointment;

public sealed class AdminCreateAppointmentCommandValidator : AbstractValidator<AdminCreateAppointmentCommand>
{
    public AdminCreateAppointmentCommandValidator(AppointmentFormFieldsValidator fieldsValidator)
    {
        RuleFor(x => x.AdminUserId).NotEmpty();
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.Fields).SetValidator(fieldsValidator);

        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.DoctorId) || !string.IsNullOrWhiteSpace(x.LabPartnerId))
            .WithMessage("Either a doctor or a lab partner must be assigned.");
    }
}
