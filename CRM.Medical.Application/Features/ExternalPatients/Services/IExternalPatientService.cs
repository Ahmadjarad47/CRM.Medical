using CRM.Medical.Application.Features.ExternalPatients.DTOs;

namespace CRM.Medical.Application.Features.ExternalPatients.Services;

public interface IExternalPatientService
{
    Task<ExternalPatientDto> CreateAsync(
        string fullName,
        int? age,
        string gender,
        string phoneNumber,
        string? externalId,
        CancellationToken cancellationToken);

    Task LinkToDirectPatientAsync(int externalPatientId, string directPatientUserId, CancellationToken cancellationToken);
}
