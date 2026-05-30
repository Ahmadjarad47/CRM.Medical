using System.Text.Json;

namespace CRM.Medical.Application.Features.TestRequests.DTOs;

public sealed record TestRequestMedicalTestItemDto(
    int TestRequestId,
    int MedicalTestId,
    string? MedicalTestNameEn,
    JsonElement? ParameterSchema,
    IReadOnlyList<TestRequestParameterItemDto> Parameters);
