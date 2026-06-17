using CRM.Medical.Domain.Enums;

namespace CRM.Medical.API.Contracts.Admin.Store;

public sealed class SaveCouponRequest
{
    public string Code { get; set; } = default!;
    public DiscountType DiscountType { get; set; }
    public decimal Amount { get; set; }
    public decimal? MinimumSubtotal { get; set; }
    public decimal? MaximumDiscountAmount { get; set; }
    public DateTime? StartsAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
}
