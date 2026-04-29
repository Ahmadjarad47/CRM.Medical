using System.Text.Json;
using CRM.Medical.Application.Features.TestResults.DTOs;

namespace CRM.Medical.Application.Features.TestResults.Services;

public interface ITestResultService
{
    Task<IReadOnlyList<TestResultDto>> ListAsync(int? testRequestId, CancellationToken cancellationToken);

    Task<TestResultDto> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<TestResultDto> GetByTestRequestIdAsync(int testRequestId, CancellationToken cancellationToken);

    Task<TestResultDto> CreateAsync(
        int testRequestId,
        DateTime resultDate,
        JsonDocument? resultData,
        string? pdfUrl,
        string status,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        int id,
        DateTime resultDate,
        JsonDocument? resultData,
        string? pdfUrl,
        string status,
        CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
