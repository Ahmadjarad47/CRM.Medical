using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.DeleteStoreProduct;

public sealed class DeleteStoreProductCommandHandler(IStoreAdminService service)
    : IRequestHandler<DeleteStoreProductCommand>
{
    public Task Handle(DeleteStoreProductCommand request, CancellationToken cancellationToken) =>
        service.DeleteProductAsync(request.Id, cancellationToken);
}
