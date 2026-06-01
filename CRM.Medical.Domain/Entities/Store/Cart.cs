namespace CRM.Medical.Domain.Entities.Store;

public sealed class Cart : BaseEntity
{
    public int Id { get; set; }

    public string LabClientId { get; set; } = string.Empty;
    public User LabClient { get; set; } = null!;

    public int? CouponId { get; set; }
    public Coupon? Coupon { get; set; }

    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
