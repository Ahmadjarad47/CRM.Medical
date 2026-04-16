namespace CRM.Medical.Domain.Entities;

public sealed class SlideCard
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    public decimal Price { get; set; }
    public decimal Discount { get; set; }

    public DateTime ExpiryDate { get; set; }

    public string Badge { get; set; } = string.Empty;
    public string DetailPageLink { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}

