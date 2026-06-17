using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Domain.Enums;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.UpdateStoreOrderStatus;

public sealed class UpdateStoreOrderStatusCommandHandler(IStoreOrderService service)
    : IRequestHandler<UpdateStoreOrderStatusCommand, StoreOrderDetailsDto>
{
    public Task<StoreOrderDetailsDto> Handle(UpdateStoreOrderStatusCommand request, CancellationToken cancellationToken) =>
        service.UpdateStatusAsync(request.Id, request.Status, cancellationToken);
}
