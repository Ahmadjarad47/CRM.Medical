using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Appointments;
using CRM.Medical.Application.Features.Appointments.DTOs;
using CRM.Medical.Application.Features.MedicalTests;
using CRM.Medical.Domain.Constants;
using MediatR;

namespace CRM.Medical.Application.Features.Appointments.Commands.UpdateAppointmentMedicalTest;

public sealed class UpdateAppointmentMedicalTestCommandHandler(
    IAppointmentRepository appointments,
    IMedicalTestRepository medicalTests,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<UpdateAppointmentMedicalTestCommand, AppointmentDto>
{
    public async Task<AppointmentDto> Handle(
        UpdateAppointmentMedicalTestCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await appointments.GetByIdAsync(request.AppointmentId, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Appointment '{request.AppointmentId}' was not found.");

        if (request.MedicalTestId is null)
        {
            entity.MedicalTestId = null;
            entity.MedicalTestCompletionStatus = null;
        }
        else
        {
            _ = await medicalTests.GetByIdAsync(request.MedicalTestId.Value, cancellationToken)
                ?? throw new ApplicationNotFoundException($"Medical test '{request.MedicalTestId}' was not found.");

            if (await appointments.IsMedicalTestLinkedToAnotherAppointmentAsync(
                    request.MedicalTestId.Value,
                    request.AppointmentId,
                    cancellationToken))
                throw new ApplicationConflictException(
                    "This medical test is already linked to a different appointment.");

            entity.MedicalTestId = request.MedicalTestId;
            entity.MedicalTestCompletionStatus = request.MedicalTestCompletionStatus
                ?? AppointmentMedicalTestCompletionStatuses.NotStarted;
        }

        entity.UpdatedAt = dateTimeProvider.UtcNow;
        await appointments.UpdateAsync(entity, cancellationToken);

        var reloaded = await appointments.GetByIdAsync(entity.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Appointment '{entity.Id}' was not found after update.");
        return reloaded.ToDto();
    }
}
