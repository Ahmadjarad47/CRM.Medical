using CRM.Medical.Application.Features.Appointments.DTOs;
using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Features.Appointments;

internal static class AppointmentMappings
{
    public static AppointmentDto ToDto(this Appointment a) =>
        new(
            a.Id,
            a.AppointmentTypeId,
            a.AppointmentType?.Name ?? string.Empty,
            a.Name,
            a.Description,
            a.Notes,
            a.Slot,
            a.LocationType,
            a.Address,
            a.Latitude,
            a.Longitude,
            a.Status,
            a.PatientId,
            a.DoctorId,
            a.LabPartnerId,
            a.CreatedByUserId,
            a.CreatedAt,
            a.UpdatedAt);
}
