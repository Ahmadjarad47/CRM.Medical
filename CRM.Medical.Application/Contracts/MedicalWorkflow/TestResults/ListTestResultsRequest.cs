namespace CRM.Medical.API.Contracts.MedicalWorkflow.TestResults;

public sealed record ListTestResultsRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public int? TestRequestId { get; init; }
    public string? Status { get; init; }
}
