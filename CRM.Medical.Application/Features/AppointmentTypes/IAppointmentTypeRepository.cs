using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Features.AppointmentTypes;

public interface IAppointmentTypeRepository
{
    Task<AppointmentType?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task AddAsync(AppointmentType entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(AppointmentType entity, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AppointmentType>> ListActiveAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AppointmentType>> ListAllAsync(CancellationToken cancellationToken = default);

    Task<bool> NameExistsAsync(string name, int? excludeId, CancellationToken cancellationToken = default);
}
