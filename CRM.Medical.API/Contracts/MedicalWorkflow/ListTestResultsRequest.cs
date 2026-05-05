namespace CRM.Medical.API.Contracts.MedicalWorkflow;

public sealed class ListTestResultsRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Search { get; init; }
    public int? TestRequestId { get; init; }
}
