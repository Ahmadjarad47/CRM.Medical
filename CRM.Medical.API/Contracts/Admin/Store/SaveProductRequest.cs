using Microsoft.AspNetCore.Http;

namespace CRM.Medical.API.Contracts.Admin.Store;

public sealed class SaveProductRequest
{
    public int CategoryId { get; set; }
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public string? Description { get; set; }
    public IFormFile? Image { get; set; }
    public string SaleUnit { get; set; } = default!;
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public string? TopBadge { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsRecommended { get; set; }
    public bool IsBestSeller { get; set; }
    public bool IsActive { get; set; }
}
