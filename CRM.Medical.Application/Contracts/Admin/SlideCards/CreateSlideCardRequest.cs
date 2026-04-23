using Microsoft.AspNetCore.Http;

namespace CRM.Medical.API.Contracts.Admin.SlideCards;

public sealed class CreateSlideCardRequest
{
    public string Title { get; init; } = default!;
    public string Description { get; init; } = default!;
    public decimal Price { get; init; }
    public decimal Discount { get; init; }
    public DateTime ExpiryDate { get; init; }
    public string Badge { get; init; } = default!;
    public string DetailPageLink { get; init; } = default!;
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
    public IFormFile? Image { get; init; }
}
