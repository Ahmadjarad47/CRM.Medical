namespace CRM.Medical.Application.Features.ExternalPatients.DTOs;

public sealed record ExternalPatientDto(
    int Id,
    string FullName,
    int? Age,
    string Gender,
    string PhoneNumber,
    string? ExternalId,
    string? LinkedDirectPatientId,
    DateTime CreatedAt);
