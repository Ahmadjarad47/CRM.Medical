namespace CRM.Medical.Domain.Entities.Store;

public sealed class StoreSetting : BaseEntity
{
    public int Id { get; set; }

    public string AnnouncementHeader { get; set; } = string.Empty;
    public string ServiceTitle { get; set; } = string.Empty;
    public string ServiceDescription { get; set; } = string.Empty;
    public decimal DeliveryFee { get; set; }
    public string DeliveryDurationText { get; set; } = string.Empty;
    public bool CashOnDeliveryEnabled { get; set; } = true;
    public bool OnlinePaymentEnabled { get; set; }
}
