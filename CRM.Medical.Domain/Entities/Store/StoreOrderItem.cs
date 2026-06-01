namespace CRM.Medical.Domain.Entities.Store;

public sealed class StoreOrderItem : BaseEntity
{
    public int Id { get; set; }

    public int StoreOrderId { get; set; }
    public StoreOrder StoreOrder { get; set; } = null!;

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public string ProductNameSnapshot { get; set; } = string.Empty;
    public string SaleUnitSnapshot { get; set; } = string.Empty;
    public string ImageSnapshot { get; set; } = string.Empty;
    public decimal UnitPriceSnapshot { get; set; }
    public decimal? DiscountPriceSnapshot { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}
