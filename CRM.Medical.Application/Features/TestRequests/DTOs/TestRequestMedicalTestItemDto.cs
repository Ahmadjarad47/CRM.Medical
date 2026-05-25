namespace CRM.Medical.Application.Features.TestRequests.DTOs;

public sealed record TestRequestMedicalTestItemDto(
    int TestRequestId,
    int MedicalTestId,
    string? MedicalTestNameEn,
    IReadOnlyList<TestRequestParameterItemDto> Parameters);
