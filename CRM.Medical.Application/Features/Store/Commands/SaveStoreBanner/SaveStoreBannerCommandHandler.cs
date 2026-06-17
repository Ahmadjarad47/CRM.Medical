using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.SaveStoreBanner;

public sealed class SaveStoreBannerCommandHandler(IStoreAdminService service)
    : IRequestHandler<SaveStoreBannerCommand, StoreBannerDto>
{
    public Task<StoreBannerDto> Handle(SaveStoreBannerCommand request, CancellationToken cancellationToken) =>
        request.Id is null
            ? service.CreateBannerAsync(
                request.Title,
                request.ImageUrl,
                request.LinkUrl,
                request.Location,
                request.CategoryId,
                request.DisplayOrder,
                request.IsActive,
                request.StartsAt,
                request.EndsAt,
                cancellationToken)
            : service.UpdateBannerAsync(
                request.Id.Value,
                request.Title,
                request.ImageUrl,
                request.LinkUrl,
                request.Location,
                request.CategoryId,
                request.DisplayOrder,
                request.IsActive,
                request.StartsAt,
                request.EndsAt,
                cancellationToken);
}
