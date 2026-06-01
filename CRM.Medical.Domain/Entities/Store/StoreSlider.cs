using CRM.Medical.Domain.Enums;

namespace CRM.Medical.Domain.Entities.Store;

public sealed class StoreSlider : BaseEntity
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public StoreSliderType Type { get; set; } = StoreSliderType.Custom;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<StoreSliderProduct> Products { get; set; } = new List<StoreSliderProduct>();
}
