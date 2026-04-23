namespace CRM.Medical.API.Contracts.MedicalWorkflow.MedicalTests;

public sealed record ListMedicalTestsRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Category { get; init; }
    public string? Status { get; init; }
}
