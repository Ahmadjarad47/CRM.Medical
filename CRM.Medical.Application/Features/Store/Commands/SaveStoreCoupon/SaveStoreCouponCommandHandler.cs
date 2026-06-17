using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using CRM.Medical.Domain.Enums;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.SaveStoreCoupon;

public sealed class SaveStoreCouponCommandHandler(IStoreAdminService service)
    : IRequestHandler<SaveStoreCouponCommand, CouponDto>
{
    public Task<CouponDto> Handle(SaveStoreCouponCommand request, CancellationToken cancellationToken) =>
        request.Id is null
            ? service.CreateCouponAsync(
                request.Code,
                request.DiscountType,
                request.Amount,
                request.MinimumSubtotal,
                request.MaximumDiscountAmount,
                request.StartsAt,
                request.ExpiresAt,
                request.IsActive,
                cancellationToken)
            : service.UpdateCouponAsync(
                request.Id.Value,
                request.Code,
                request.DiscountType,
                request.Amount,
                request.MinimumSubtotal,
                request.MaximumDiscountAmount,
                request.StartsAt,
                request.ExpiresAt,
                request.IsActive,
                cancellationToken);
}
