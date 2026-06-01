namespace CRM.Medical.Domain.Entities.ServiceRequests;

public sealed class ClientJoinRequest : BaseEntity
{
    public int Id { get; set; }

    public string ManagerName { get; set; } = string.Empty;

    public string LabName { get; set; } = string.Empty;

    public string MobileNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string? AdditionalInfo { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? Notes { get; set; }
}
