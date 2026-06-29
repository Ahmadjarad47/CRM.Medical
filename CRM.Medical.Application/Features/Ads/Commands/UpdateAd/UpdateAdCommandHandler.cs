using CRM.Medical.Application.Common.Storage;
using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Ads.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Ads.Commands.UpdateAd;

public sealed class UpdateAdCommandHandler(
    IAdRepository ads,
    IFileStorageService fileStorage,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<UpdateAdCommand, AdDto>
{
    public async Task<AdDto> Handle(UpdateAdCommand request, CancellationToken cancellationToken)
    {
        var entity = await ads.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Ad '{request.Id}' was not found.");

        if (request.Media is { Length: > 0 })
            entity.MediaUrl = await fileStorage.UploadFileAsync(request.Media, "ads", cancellationToken);

        entity.Name = request.Name.Trim();
        entity.Description = request.Description.Trim();
        entity.Latitude = request.Latitude;
        entity.Longitude = request.Longitude;
        entity.AddressName = request.AddressName.Trim();
        entity.MediaType = request.MediaType;
        entity.DisplayMode = request.DisplayMode;
        entity.UpdatedAt = dateTimeProvider.UtcNow;

        await ads.UpdateAsync(entity, cancellationToken);
        return entity.ToDto();
    }
}
