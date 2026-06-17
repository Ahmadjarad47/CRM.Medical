using Microsoft.AspNetCore.Http;

namespace CRM.Medical.API.Contracts.Admin.Store;

public sealed class SaveProductCategoryRequest
{
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public string? Description { get; set; }
    public IFormFile? Image { get; set; }
    public int? ParentCategoryId { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}
