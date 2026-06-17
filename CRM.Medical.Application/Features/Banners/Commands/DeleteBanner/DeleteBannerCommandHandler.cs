using CRM.Medical.Application.Exceptions;
using MediatR;

namespace CRM.Medical.Application.Features.Banners.Commands.DeleteBanner;

public sealed class DeleteBannerCommandHandler(IBannerRepository banners)
    : IRequestHandler<DeleteBannerCommand>
{
    public async Task Handle(DeleteBannerCommand request, CancellationToken cancellationToken)
    {
        var entity = await banners.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Banner '{request.Id}' was not found.");

        await banners.DeleteAsync(entity, cancellationToken);
    }
}
