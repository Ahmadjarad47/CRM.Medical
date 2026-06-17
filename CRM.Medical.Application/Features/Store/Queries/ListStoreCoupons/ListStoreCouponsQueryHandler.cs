using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Queries.ListStoreCoupons;

public sealed class ListStoreCouponsQueryHandler(IStoreAdminService service)
    : IRequestHandler<ListStoreCouponsQuery, IReadOnlyList<CouponDto>>
{
    public Task<IReadOnlyList<CouponDto>> Handle(ListStoreCouponsQuery request, CancellationToken cancellationToken) =>
        service.ListCouponsAsync(cancellationToken);
}
