using CRM.Medical.Domain.Enums;

namespace CRM.Medical.Domain.Entities.Store;

public sealed class PaymentTransaction : BaseEntity
{
    public int Id { get; set; }

    public int StoreOrderId { get; set; }
    public StoreOrder StoreOrder { get; set; } = null!;

    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus Status { get; set; }
    public decimal Amount { get; set; }
    public string? Provider { get; set; }
    public string? ProviderTransactionId { get; set; }
    public DateTime CreatedOnUtc { get; set; }
}
