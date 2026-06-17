namespace CRM.Medical.API.Contracts.Admin.Store;

public sealed class UpdateStoreSettingsRequest
{
    public string AnnouncementHeader { get; set; } = default!;
    public string ServiceTitle { get; set; } = default!;
    public string ServiceDescription { get; set; } = default!;
    public decimal DeliveryFee { get; set; }
    public string DeliveryDurationText { get; set; } = default!;
    public bool CashOnDeliveryEnabled { get; set; }
    public bool OnlinePaymentEnabled { get; set; }
}
