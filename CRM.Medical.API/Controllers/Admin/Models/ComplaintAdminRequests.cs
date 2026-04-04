namespace CRM.Medical.API.Controllers.Admin.Models;

public sealed class ListComplaintsRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Status { get; init; }
    public string? UserId { get; init; }
}

public sealed class UpdateComplaintStatusRequest
{
    public string Status { get; init; } = string.Empty;
}
