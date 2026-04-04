using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Features.AppointmentTypes;
using CRM.Medical.Application.Features.Appointments;
using CRM.Medical.Application.Features.Appointments.DTOs;
using CRM.Medical.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CRM.Medical.Application.Features.Appointments.Commands.DoctorCreateAppointment;

public sealed class DoctorCreateAppointmentCommandHandler(
    IAppointmentRepository appointments,
    IAppointmentTypeRepository appointmentTypes,
    UserManager<User> userManager,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<DoctorCreateAppointmentCommand, AppointmentDto>
{
    public async Task<AppointmentDto> Handle(
        DoctorCreateAppointmentCommand request,
        CancellationToken cancellationToken)
    {
        await AppointmentIdentityRules.EnsurePatientAsync(userManager, request.PatientId, cancellationToken);
        await AppointmentIdentityRules.EnsureLabPartnerAsync(userManager, request.LabPartnerId, cancellationToken);

        await AppointmentTypeRules.EnsureActiveTypeAsync(
            appointmentTypes,
            request.Fields.AppointmentTypeId,
            cancellationToken);

        var now = dateTimeProvider.UtcNow;
        var f = request.Fields;
        var entity = new Appointment
        {
            AppointmentTypeId = f.AppointmentTypeId,
            Name = f.Name,
            Description = f.Description,
            Notes = f.Notes,
            Slot = f.Slot,
            LocationType = f.LocationType,
            Address = f.Address,
            Latitude = f.Latitude,
            Longitude = f.Longitude,
            Status = AppointmentStatuses.Pending,
            PatientId = request.PatientId,
            DoctorId = request.DoctorUserId,
            LabPartnerId = request.LabPartnerId,
            CreatedByUserId = request.DoctorUserId,
            CreatedAt = now
        };

        await appointments.AddAsync(entity, cancellationToken);
        var created = await appointments.GetByIdAsync(entity.Id, cancellationToken);
        return created!.ToDto();
    }
}
