using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.DeleteStoreBanner;

public sealed class DeleteStoreBannerCommandHandler(IStoreAdminService service)
    : IRequestHandler<DeleteStoreBannerCommand>
{
    public Task Handle(DeleteStoreBannerCommand request, CancellationToken cancellationToken) =>
        service.DeleteBannerAsync(request.Id, cancellationToken);
}
