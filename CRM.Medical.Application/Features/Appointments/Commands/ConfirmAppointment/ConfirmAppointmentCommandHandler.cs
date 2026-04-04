using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Appointments;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.Commands.ConfirmAppointment;

public sealed class ConfirmAppointmentCommandHandler(
    IAppointmentRepository appointments,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<ConfirmAppointmentCommand>
{
    public async Task Handle(ConfirmAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await appointments.GetByIdAsync(request.AppointmentId, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Appointment '{request.AppointmentId}' was not found.");

        if (appointment.Status != AppointmentStatuses.Pending)
            throw new ApplicationBadRequestException("Only pending appointments can be confirmed.");

        var allowed = request.Actor switch
        {
            AppointmentConfirmationActor.Admin => true,
            AppointmentConfirmationActor.Doctor => string.Equals(
                appointment.DoctorId,
                request.ActingUserId,
                StringComparison.Ordinal),
            AppointmentConfirmationActor.LabPartner => string.Equals(
                appointment.LabPartnerId,
                request.ActingUserId,
                StringComparison.Ordinal),
            _ => false
        };

        if (!allowed)
            throw new ApplicationForbiddenException("You are not allowed to confirm this appointment.");

        appointment.Status = AppointmentStatuses.Confirmed;
        appointment.UpdatedAt = dateTimeProvider.UtcNow;
        await appointments.UpdateAsync(appointment, cancellationToken);
    }
}
