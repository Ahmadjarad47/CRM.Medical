using System.Text.Json;

namespace CRM.Medical.Application.Features.TestRequests.DTOs;

public sealed record TestRequestDto(
    int Id,
    int MedicalTestId,
    string MedicalTestNameEn,
    DateTime RequestDate,
    string Status,
    double TotalAmount,
    string? Notes,
    JsonElement? Metadata,
    string CreatedByUserId,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int? ResultId);
