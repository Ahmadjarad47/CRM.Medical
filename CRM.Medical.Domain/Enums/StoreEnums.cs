namespace CRM.Medical.Domain.Enums;

public enum StoreOrderStatus
{
    Pending = 1,
    Confirmed = 2,
    Preparing = 3,
    OutForDelivery = 4,
    Delivered = 5,
    Cancelled = 6
}

public enum PaymentMethod
{
    CashOnDelivery = 1,
    Online = 2
}

public enum PaymentStatus
{
    Pending = 1,
    Paid = 2,
    Failed = 3,
    Refunded = 4
}

public enum DiscountType
{
    Percentage = 1,
    FixedAmount = 2
}

public enum StoreSliderType
{
    Custom = 1,
    NewArrivals = 2,
    Offers = 3,
    Recommended = 4,
    BestSellers = 5
}
