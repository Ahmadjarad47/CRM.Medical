using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.RemoveStoreCoupon;

public sealed class RemoveStoreCouponCommandHandler(ICartService service)
    : IRequestHandler<RemoveStoreCouponCommand, CartDto>
{
    public Task<CartDto> Handle(RemoveStoreCouponCommand request, CancellationToken cancellationToken) =>
        service.RemoveCouponAsync(request.LabClientId, cancellationToken);
}
