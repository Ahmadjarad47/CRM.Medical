using Microsoft.AspNetCore.Http;

namespace CRM.Medical.API.Contracts.Admin.Store;

public sealed class SaveStoreBannerRequest
{
    public string Title { get; set; } = default!;
    public IFormFile? Image { get; set; }
    public string? LinkUrl { get; set; }
    public string Location { get; set; } = default!;
    public int? CategoryId { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime? StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
}
