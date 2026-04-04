using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Appointments;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.Commands.CancelAppointment;

public sealed class CancelAppointmentCommandHandler(
    IAppointmentRepository appointments,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<CancelAppointmentCommand>
{
    public async Task Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await appointments.GetByIdAsync(request.AppointmentId, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Appointment '{request.AppointmentId}' was not found.");

        if (appointment.Status == AppointmentStatuses.Cancelled)
            throw new ApplicationBadRequestException("This appointment is already cancelled.");

        var allowed = request.Actor switch
        {
            AppointmentCancellationActor.Admin => true,
            AppointmentCancellationActor.Patient => string.Equals(
                appointment.PatientId,
                request.ActingUserId,
                StringComparison.Ordinal),
            AppointmentCancellationActor.Doctor => string.Equals(
                appointment.DoctorId,
                request.ActingUserId,
                StringComparison.Ordinal),
            AppointmentCancellationActor.LabPartner => string.Equals(
                appointment.LabPartnerId,
                request.ActingUserId,
                StringComparison.Ordinal),
            _ => false
        };

        if (!allowed)
            throw new ApplicationForbiddenException("You are not allowed to cancel this appointment.");

        appointment.Status = AppointmentStatuses.Cancelled;
        appointment.UpdatedAt = dateTimeProvider.UtcNow;
        await appointments.UpdateAsync(appointment, cancellationToken);
    }
}
