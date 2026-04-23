namespace CRM.Medical.API.Contracts.Admin.Complaints;

public sealed class ListComplaintsRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Status { get; init; }
    public string? UserId { get; init; }
}
