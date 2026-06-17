using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.SaveStoreSlider;

public sealed class SaveStoreSliderCommandHandler(IStoreAdminService service)
    : IRequestHandler<SaveStoreSliderCommand, StoreSliderDto>
{
    public Task<StoreSliderDto> Handle(SaveStoreSliderCommand request, CancellationToken cancellationToken) =>
        request.Id is null
            ? service.CreateSliderAsync(
                request.Title,
                request.Type,
                request.DisplayOrder,
                request.IsActive,
                request.ProductIds,
                cancellationToken)
            : service.UpdateSliderAsync(
                request.Id.Value,
                request.Title,
                request.Type,
                request.DisplayOrder,
                request.IsActive,
                request.ProductIds,
                cancellationToken);
}
