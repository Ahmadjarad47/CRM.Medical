namespace CRM.Medical.Domain.Entities.Store;

public sealed class ProductCategory : BaseEntity
{
    public int Id { get; set; }

    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    public int? ParentCategoryId { get; set; }
    public ProductCategory? ParentCategory { get; set; }
    public ICollection<ProductCategory> Subcategories { get; set; } = new List<ProductCategory>();

    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<StoreBanner> Banners { get; set; } = new List<StoreBanner>();
}
