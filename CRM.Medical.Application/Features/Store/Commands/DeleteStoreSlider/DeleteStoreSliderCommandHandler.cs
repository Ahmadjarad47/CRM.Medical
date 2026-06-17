using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.DeleteStoreSlider;

public sealed class DeleteStoreSliderCommandHandler(IStoreAdminService service)
    : IRequestHandler<DeleteStoreSliderCommand>
{
    public Task Handle(DeleteStoreSliderCommand request, CancellationToken cancellationToken) =>
        service.DeleteSliderAsync(request.Id, cancellationToken);
}
