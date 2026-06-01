using CRM.Medical.Domain.Enums;

namespace CRM.Medical.Domain.Entities.Insurance;

public sealed class InsuranceApprovalRequest : BaseEntity
{
    public int Id { get; set; }

    public string PatientId { get; set; } = string.Empty;

    public User Patient { get; set; } = null!;

    public string InsuredName { get; set; } = string.Empty;

    public string InsuranceNumber { get; set; } = string.Empty;

    public string MobileNumber { get; set; } = string.Empty;

    public string InsuranceCardImageUrl { get; set; } = string.Empty;

    public string InsuranceCardOriginalFileName { get; set; } = string.Empty;

    public string PrescriptionImageUrl { get; set; } = string.Empty;

    public string PrescriptionOriginalFileName { get; set; } = string.Empty;

    public InsuranceApprovalRequestStatus Status { get; set; } = InsuranceApprovalRequestStatus.New;

    public string? Notes { get; set; }

    public string? RejectionReason { get; set; }
}
