using System.Text.Json;

namespace CRM.Medical.Application.Features.MedicalTestResults.DTOs;

public sealed record TestResultDto(
    int Id,
    int TestRequestId,
    int MedicalTestId,
    string MedicalTestNameEn,
    DateTime ResultDate,
    JsonElement? ResultData,
    string? PdfUrl,
    string Status,
    string CreatedByUserId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
