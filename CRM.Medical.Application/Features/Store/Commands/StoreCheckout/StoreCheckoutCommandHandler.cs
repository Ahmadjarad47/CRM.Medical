using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Domain.Enums;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.StoreCheckout;

public sealed class StoreCheckoutCommandHandler(ICheckoutService service)
    : IRequestHandler<StoreCheckoutCommand, StoreOrderDetailsDto>
{
    public Task<StoreOrderDetailsDto> Handle(StoreCheckoutCommand request, CancellationToken cancellationToken) =>
        service.CheckoutAsync(request.LabClientId, request.PaymentMethod, request.Notes, cancellationToken);
}
