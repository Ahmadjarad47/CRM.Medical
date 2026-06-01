namespace CRM.Medical.Domain.Entities.Accounting;

public sealed class LabAccountPayment : BaseEntity
{
    public int Id { get; set; }

    public string LabClientId { get; set; } = string.Empty;
    public User LabClient { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateTime PaidAt { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;

    public string? ReferenceNumber { get; set; }

    public string? Notes { get; set; }
}
