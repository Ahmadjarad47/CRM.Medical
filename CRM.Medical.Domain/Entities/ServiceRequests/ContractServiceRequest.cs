using CRM.Medical.Domain.Enums;

namespace CRM.Medical.Domain.Entities.ServiceRequests;

public sealed class ContractServiceRequest : BaseEntity
{
    public int Id { get; set; }

    public ContractRequestType ContractType { get; set; }

    public string ResponsibleName { get; set; } = string.Empty;

    public string? OrganizationName { get; set; }

    public int ExpectedSubscribersCount { get; set; }

    public string ContactNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public ContractDuration ContractDuration { get; set; }

    public string? AdditionalInfo { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? Notes { get; set; }
}
