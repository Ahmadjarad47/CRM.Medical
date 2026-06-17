using CRM.Medical.Domain.Enums;

namespace CRM.Medical.API.Contracts.Admin.Store;

public sealed class SaveStoreSliderRequest
{
    public string Title { get; set; } = default!;
    public StoreSliderType Type { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public List<int> ProductIds { get; set; } = [];
}
