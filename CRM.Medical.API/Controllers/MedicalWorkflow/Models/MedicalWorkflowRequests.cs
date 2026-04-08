using System.Text.Json;

namespace CRM.Medical.API.Controllers.MedicalWorkflow.Models;

public sealed record ListMedicalTestsRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Category { get; init; }
    public string? Status { get; init; }
}

public sealed record CreateMedicalTestRequest(
    string NameAr,
    string NameEn,
    double Price,
    string Category,
    string SampleType,
    JsonElement? ParameterSchema,
    string Status);

public sealed record UpdateMedicalTestRequest(
    string NameAr,
    string NameEn,
    double Price,
    string Category,
    string SampleType,
    JsonElement? ParameterSchema,
    string Status);

public sealed record ListTestRequestsRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public int? MedicalTestId { get; init; }
    public string? Status { get; init; }
}

public sealed record CreateTestRequestRequest(
    int MedicalTestId,
    DateTime RequestDate,
    string Status,
    double TotalAmount,
    string? Notes,
    JsonElement? Metadata);

public sealed record UpdateTestRequestRequest(
    DateTime RequestDate,
    string Status,
    double TotalAmount,
    string? Notes,
    JsonElement? Metadata);

public sealed record ListTestResultsRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public int? TestRequestId { get; init; }
    public string? Status { get; init; }
}

public sealed record CreateTestResultRequest(
    int TestRequestId,
    DateTime ResultDate,
    JsonElement? ResultData,
    string? PdfUrl,
    string Status);

public sealed record UpdateTestResultRequest(
    DateTime ResultDate,
    JsonElement? ResultData,
    string? PdfUrl,
    string Status);

public sealed record UpdateAppointmentMedicalTestRequest
{
    public int? MedicalTestId { get; init; }
    public string? MedicalTestCompletionStatus { get; init; }
}
