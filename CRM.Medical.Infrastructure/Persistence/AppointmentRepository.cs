using CRM.Medical.Application.Features.Appointments;
using CRM.Medical.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM.Medical.Infrastructure.Persistence;

public sealed class AppointmentRepository(MedicalDbContext dbContext) : IAppointmentRepository
{
    public async Task<Appointment> AddAsync(Appointment entity, CancellationToken cancellationToken = default)
    {
        dbContext.Appointments.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public Task<Appointment?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        dbContext.Appointments
            .Include(a => a.AppointmentType)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<Appointment> Items, int TotalCount)> ListForPatientAsync(
        string patientId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Appointments.AsNoTracking()
            .Include(a => a.AppointmentType)
            .Where(a => a.PatientId == patientId);
        return await ListPagedAsync(query, page, pageSize, cancellationToken);
    }

    public async Task<(IReadOnlyList<Appointment> Items, int TotalCount)> ListForDoctorAsync(
        string doctorId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Appointments.AsNoTracking()
            .Include(a => a.AppointmentType)
            .Where(a => a.DoctorId == doctorId);
        return await ListPagedAsync(query, page, pageSize, cancellationToken);
    }

    public async Task<(IReadOnlyList<Appointment> Items, int TotalCount)> ListForLabAsync(
        string labPartnerId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Appointments.AsNoTracking()
            .Include(a => a.AppointmentType)
            .Where(a => a.LabPartnerId == labPartnerId);
        return await ListPagedAsync(query, page, pageSize, cancellationToken);
    }

    public async Task<(IReadOnlyList<Appointment> Items, int TotalCount)> ListAdminAsync(
        string? patientId,
        string? doctorId,
        string? labPartnerId,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Appointment> query = dbContext.Appointments.AsNoTracking()
            .Include(a => a.AppointmentType);

        if (!string.IsNullOrEmpty(patientId))
            query = query.Where(a => a.PatientId == patientId);

        if (!string.IsNullOrEmpty(doctorId))
            query = query.Where(a => a.DoctorId == doctorId);

        if (!string.IsNullOrEmpty(labPartnerId))
            query = query.Where(a => a.LabPartnerId == labPartnerId);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(a => a.Status == status);

        return await ListPagedAsync(query, page, pageSize, cancellationToken);
    }

    public async Task UpdateAsync(Appointment entity, CancellationToken cancellationToken = default)
    {
        dbContext.Appointments.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task<(IReadOnlyList<Appointment> Items, int TotalCount)> ListPagedAsync(
        IQueryable<Appointment> query,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(a => a.Slot)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return (items, total);
    }
}
