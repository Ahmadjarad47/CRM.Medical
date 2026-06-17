using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.ApplyStoreCoupon;

public sealed class ApplyStoreCouponCommandHandler(ICartService service)
    : IRequestHandler<ApplyStoreCouponCommand, CartDto>
{
    public Task<CartDto> Handle(ApplyStoreCouponCommand request, CancellationToken cancellationToken) =>
        service.ApplyCouponAsync(request.LabClientId, request.Code, cancellationToken);
}
