using System.Text.Json;

namespace CRM.Medical.Application.Features.TestResults.DTOs;

public sealed record TestResultDto(
    int Id,
    int TestRequestId,
    DateTime ResultDate,
    JsonElement? ResultData,
    string? PdfUrl,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
