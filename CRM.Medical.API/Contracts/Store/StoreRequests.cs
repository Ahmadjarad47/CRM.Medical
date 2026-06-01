using CRM.Medical.Domain.Enums;

namespace CRM.Medical.API.Contracts.Store;

public sealed record AddCartItemRequest(int ProductId, int Quantity);

public sealed record UpdateCartItemRequest(int Quantity);

public sealed record ApplyCouponRequest(string Code);

public sealed record CheckoutRequest(PaymentMethod PaymentMethod, string? Notes);
