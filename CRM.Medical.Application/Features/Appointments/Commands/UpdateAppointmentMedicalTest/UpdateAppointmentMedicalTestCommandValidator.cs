using CRM.Medical.Domain.Constants;
using FluentValidation;

namespace CRM.Medical.Application.Features.Appointments.Commands.UpdateAppointmentMedicalTest;

public sealed class UpdateAppointmentMedicalTestCommandValidator
    : AbstractValidator<UpdateAppointmentMedicalTestCommand>
{
    public UpdateAppointmentMedicalTestCommandValidator()
    {
        RuleFor(x => x.AppointmentId).GreaterThan(0);

        RuleFor(x => x)
            .Must(c => c.MedicalTestId is not null || c.MedicalTestCompletionStatus is null)
            .WithMessage("MedicalTestCompletionStatus must be null when MedicalTestId is null.");

        RuleFor(x => x.MedicalTestCompletionStatus)
            .Must(s => s is null || AppointmentMedicalTestCompletionStatuses.All.Contains(s))
            .WithMessage("MedicalTestCompletionStatus must be a known completion value.")
            .When(x => x.MedicalTestCompletionStatus is not null);
    }
}
