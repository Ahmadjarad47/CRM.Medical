using System.Text.Json;
using CRM.Medical.Application.Features.TestRequests.DTOs;

namespace CRM.Medical.Application.Features.TestRequests.Services;

public interface ITestRequestService
{
    Task<IReadOnlyList<TestRequestDto>> ListAsync(CancellationToken cancellationToken);

    Task<TestRequestDto> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<TestRequestDto> CreateAsync(
        int medicalTestId,
        DateTime requestDate,
        string status,
        double totalAmount,
        string? notes,
        JsonDocument? metadata,
        string? doctorId,
        string? labClientId,
        string? directPatientId,
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
        CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
