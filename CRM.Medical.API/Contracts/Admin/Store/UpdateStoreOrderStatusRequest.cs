using CRM.Medical.Domain.Enums;

namespace CRM.Medical.API.Contracts.Admin.Store;

public sealed class UpdateStoreOrderStatusRequest
{
    public StoreOrderStatus Status { get; set; }
}
