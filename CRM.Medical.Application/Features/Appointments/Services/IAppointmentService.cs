using CRM.Medical.Application.Features.Appointments.DTOs;

namespace CRM.Medical.Application.Features.Appointments.Services;

public interface IAppointmentService
{
    Task<IReadOnlyList<AppointmentDto>> ListAsync(
        DateTime? fromUtc,
        DateTime? toUtc,
        string? userId,
        string? status,
        CancellationToken cancellationToken);

    Task<AppointmentDto> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<AppointmentDto> CreateAsync(
        int availabilityId,
        int? testRequestId,
        DateTime startTimeUtc,
        DateTime endTimeUtc,
        string patientLocationType,
        int? age,
        string? gender,
        double? patientLatitude,
        double? patientLongitude,
        string? notes,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        int id,
        int availabilityId,
        int testRequestId,
        string? userId,
        string patientLocationType,
        int? age,
        string? gender,
        double? patientLatitude,
        double? patientLongitude,
        string? notes,
        CancellationToken cancellationToken);

    Task CancelAsync(int id, CancellationToken cancellationToken);

    Task<AppointmentDayAvailabilityDto> GetDayAvailabilityAsync(
        DateTime date,
        string? userId,
        CancellationToken cancellationToken);
}
