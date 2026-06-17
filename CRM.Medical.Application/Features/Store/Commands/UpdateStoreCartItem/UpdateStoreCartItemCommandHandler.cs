using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.UpdateStoreCartItem;

public sealed class UpdateStoreCartItemCommandHandler(ICartService service)
    : IRequestHandler<UpdateStoreCartItemCommand, CartDto>
{
    public Task<CartDto> Handle(UpdateStoreCartItemCommand request, CancellationToken cancellationToken) =>
        service.UpdateItemAsync(request.LabClientId, request.ItemId, request.Quantity, cancellationToken);
}
