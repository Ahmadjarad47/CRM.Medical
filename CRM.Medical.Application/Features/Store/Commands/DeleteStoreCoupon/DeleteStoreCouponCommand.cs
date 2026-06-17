using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.DeleteStoreCoupon;

public sealed record DeleteStoreCouponCommand(int Id) : IRequest;
