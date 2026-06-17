using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.DeleteStoreCategory;

public sealed class DeleteStoreCategoryCommandHandler(IStoreAdminService service)
    : IRequestHandler<DeleteStoreCategoryCommand>
{
    public Task Handle(DeleteStoreCategoryCommand request, CancellationToken cancellationToken) =>
        service.DeleteCategoryAsync(request.Id, cancellationToken);
}
