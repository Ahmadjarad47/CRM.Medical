using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.RemoveStoreCartItem;

public sealed class RemoveStoreCartItemCommandHandler(ICartService service)
    : IRequestHandler<RemoveStoreCartItemCommand>
{
    public Task Handle(RemoveStoreCartItemCommand request, CancellationToken cancellationToken) =>
        service.RemoveItemAsync(request.LabClientId, request.ItemId, cancellationToken);
}
