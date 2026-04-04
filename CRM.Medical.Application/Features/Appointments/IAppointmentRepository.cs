using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Features.Appointments;

public interface IAppointmentRepository
{
    Task<Appointment> AddAsync(Appointment entity, CancellationToken cancellationToken = default);

    Task<Appointment?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Appointment> Items, int TotalCount)> ListForPatientAsync(
        string patientId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Appointment> Items, int TotalCount)> ListForDoctorAsync(
        string doctorId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Appointment> Items, int TotalCount)> ListForLabAsync(
        string labPartnerId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Appointment> Items, int TotalCount)> ListAdminAsync(
        string? patientId,
        string? doctorId,
        string? labPartnerId,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(Appointment entity, CancellationToken cancellationToken = default);
}
