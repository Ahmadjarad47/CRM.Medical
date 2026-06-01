namespace CRM.Medical.Domain.Entities.Store;

public sealed class StoreSliderProduct
{
    public int Id { get; set; }

    public int StoreSliderId { get; set; }
    public StoreSlider StoreSlider { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int DisplayOrder { get; set; }
}
