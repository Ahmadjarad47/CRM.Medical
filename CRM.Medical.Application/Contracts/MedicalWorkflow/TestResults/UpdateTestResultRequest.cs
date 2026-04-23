using Microsoft.AspNetCore.Http;

namespace CRM.Medical.API.Contracts.MedicalWorkflow.TestResults;

public sealed class UpdateTestResultRequest
{
    public DateTime ResultDate { get; init; }
    public string? ResultDataJson { get; init; }
    public string Status { get; init; } = string.Empty;
    public IFormFile? PdfFile { get; init; }
}
