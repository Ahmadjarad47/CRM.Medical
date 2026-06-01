using CRM.Medical.Domain.Enums;

namespace CRM.Medical.Domain.Entities.Store;

public sealed class StoreOrder : BaseEntity
{
    public int Id { get; set; }

    public string OrderNumber { get; set; } = string.Empty;
    public string LabClientId { get; set; } = string.Empty;
    public User LabClient { get; set; } = null!;

    public StoreOrderStatus Status { get; set; } = StoreOrderStatus.Pending;
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CashOnDelivery;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal Total { get; set; }

    public int? CouponId { get; set; }
    public Coupon? Coupon { get; set; }
    public string? CouponCodeSnapshot { get; set; }

    public string DeliveryDurationSnapshot { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime OrderedAt { get; set; }

    public ICollection<StoreOrderItem> Items { get; set; } = new List<StoreOrderItem>();
    public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
}
