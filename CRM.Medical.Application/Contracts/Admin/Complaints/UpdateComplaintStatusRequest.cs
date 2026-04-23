namespace CRM.Medical.API.Contracts.Admin.Complaints;

public sealed class UpdateComplaintStatusRequest
{
    public string Status { get; init; } = string.Empty;
}
