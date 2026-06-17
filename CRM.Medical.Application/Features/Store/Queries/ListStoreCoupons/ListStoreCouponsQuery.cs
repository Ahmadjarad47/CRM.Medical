using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Queries.ListStoreCoupons;

public sealed record ListStoreCouponsQuery : IRequest<IReadOnlyList<CouponDto>>;
