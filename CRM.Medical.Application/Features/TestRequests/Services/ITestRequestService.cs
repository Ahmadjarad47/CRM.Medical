using CRM.Medical.Application.Common.Responses;
using System.Text.Json;
using CRM.Medical.Application.Features.TestRequests.DTOs;

namespace CRM.Medical.Application.Features.TestRequests.Services;

public interface ITestRequestService
{
    Task<PagedResult<TestRequestDto>> ListAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken);

    Task<TestRequestDto> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<IReadOnlyList<TestRequestDto>> CreateAsync(
        IReadOnlyList<int> medicalTestIds,
        DateTime requestDate,
        string status,
        double totalAmount,
        string? notes,
        JsonDocument? metadata,
        string? doctorId,
        string? labClientId,
        string? directPatientId,
        int? externalPatientId,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        int id,
        DateTime requestDate,
        string status,
        double totalAmount,
        string? notes,
        JsonDocument? metadata,
        string? doctorId,
        string? labClientId,
        string? directPatientId,
        int? externalPatientId,
        CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
