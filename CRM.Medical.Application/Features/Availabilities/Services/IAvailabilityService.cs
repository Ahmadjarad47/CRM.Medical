using CRM.Medical.Application.Features.Availabilities.DTOs;

namespace CRM.Medical.Application.Features.Availabilities.Services;

public interface IAvailabilityService
{
    Task<IReadOnlyList<AvailabilityDto>> ListAsync(string? userId, CancellationToken cancellationToken);

    Task<AvailabilityDto> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<AvailabilityDto> CreateAsync(
        DayOfWeek dayOfWeek,
        TimeSpan startTime,
        TimeSpan endTime,
        int slotDuration,
        bool isActive,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        int id,
        DayOfWeek dayOfWeek,
        TimeSpan startTime,
        TimeSpan endTime,
        int slotDuration,
        bool isActive,
        CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
