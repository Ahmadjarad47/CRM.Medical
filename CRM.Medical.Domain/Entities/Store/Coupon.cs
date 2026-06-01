using CRM.Medical.Domain.Enums;

namespace CRM.Medical.Domain.Entities.Store;

public sealed class Coupon : BaseEntity
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;
    public DiscountType DiscountType { get; set; }
    public decimal Amount { get; set; }
    public decimal? MinimumSubtotal { get; set; }
    public decimal? MaximumDiscountAmount { get; set; }
    public DateTime? StartsAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Cart> Carts { get; set; } = new List<Cart>();
    public ICollection<StoreOrder> Orders { get; set; } = new List<StoreOrder>();
}
