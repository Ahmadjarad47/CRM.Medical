using System.Text.Json;

namespace CRM.Medical.Application.Features.TestRequests.DTOs;

public sealed record TestRequestDto(
    int Id,
    int MedicalTestId,
    string? MedicalTestNameEn,
    string? DoctorId,
    string? LabClientId,
    string? DirectPatientId,
    DateTime RequestDate,
    string Status,
    double TotalAmount,
    string? Notes,
    JsonElement? Metadata,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
