namespace CRM.Medical.Domain.Entities.Store;

public sealed class StoreBanner : BaseEntity
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string? LinkUrl { get; set; }
    public string Location { get; set; } = string.Empty;

    public int? CategoryId { get; set; }
    public ProductCategory? Category { get; set; }

    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
}
