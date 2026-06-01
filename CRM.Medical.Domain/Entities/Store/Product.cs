namespace CRM.Medical.Domain.Entities.Store;

public sealed class Product : BaseEntity
{
    public int Id { get; set; }

    public int CategoryId { get; set; }
    public ProductCategory Category { get; set; } = null!;

    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string SaleUnit { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public string? TopBadge { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsRecommended { get; set; }
    public bool IsBestSeller { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<StoreSliderProduct> SliderProducts { get; set; } = new List<StoreSliderProduct>();
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
