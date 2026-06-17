using CRM.Medical.Application.Common.Storage;
using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Features.Ads.DTOs;
using CRM.Medical.Domain.Entities;
using MediatR;

namespace CRM.Medical.Application.Features.Ads.Commands.CreateAd;

public sealed class CreateAdCommandHandler(
    IAdRepository ads,
    IFileStorageService fileStorage,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<CreateAdCommand, AdDto>
{
    public async Task<AdDto> Handle(CreateAdCommand request, CancellationToken cancellationToken)
    {
        var mediaUrl = await fileStorage.UploadFileAsync(request.Media, "ads", cancellationToken);

        var entity = new Ad
        {
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            MediaType = request.MediaType,
            MediaUrl = mediaUrl,
            CreatedAt = dateTimeProvider.UtcNow
        };

        await ads.AddAsync(entity, cancellationToken);
        return entity.ToDto();
    }
}
