using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Domain.Enums;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.SaveStoreCoupon;

public sealed record SaveStoreCouponCommand(
    int? Id,
    string Code,
    DiscountType DiscountType,
    decimal Amount,
    decimal? MinimumSubtotal,
    decimal? MaximumDiscountAmount,
    DateTime? StartsAt,
    DateTime? ExpiresAt,
    bool IsActive) : IRequest<CouponDto>;
