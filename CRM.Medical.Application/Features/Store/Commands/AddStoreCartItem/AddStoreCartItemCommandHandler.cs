using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.AddStoreCartItem;

public sealed class AddStoreCartItemCommandHandler(ICartService service)
    : IRequestHandler<AddStoreCartItemCommand, CartDto>
{
    public Task<CartDto> Handle(AddStoreCartItemCommand request, CancellationToken cancellationToken) =>
        service.AddItemAsync(request.LabClientId, request.ProductId, request.Quantity, cancellationToken);
}
