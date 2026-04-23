namespace CRM.Medical.API.Contracts.MedicalWorkflow.TestRequests;

public sealed record ListTestRequestsRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public int? MedicalTestId { get; init; }
    public string? Status { get; init; }
}
