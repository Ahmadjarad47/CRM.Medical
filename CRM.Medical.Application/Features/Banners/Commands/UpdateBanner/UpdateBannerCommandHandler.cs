using System.Text.Json;
using CRM.Medical.Application.Common.Storage;
using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Banners.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Banners.Commands.UpdateBanner;

public sealed class UpdateBannerCommandHandler(
    IBannerRepository banners,
    IFileStorageService fileStorage,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<UpdateBannerCommand, BannerDto>
{
    public async Task<BannerDto> Handle(
        UpdateBannerCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await banners.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Banner '{request.Id}' was not found.");

        if (request.Media is { Length: > 0 })
            entity.MediaUrl = await fileStorage.UploadFileAsync(request.Media, "banners", cancellationToken);

        if (request.VisibilityRules is { } json)
        {
            entity.VisibilityRules?.Dispose();
            entity.VisibilityRules = JsonDocument.Parse(json.GetRawText());
        }

        entity.Title = request.Title.Trim();
        entity.DisplayMode = request.DisplayMode;
        entity.InternalLink = request.InternalLink?.Trim() ?? string.Empty;
        entity.ExternalLink = request.ExternalLink?.Trim() ?? string.Empty;
        entity.TargetType = request.TargetType.Trim();
        entity.Location = request.Location.Trim();
        entity.DisplayOrder = request.DisplayOrder;
        entity.IsActive = request.IsActive;
        entity.StartDate = request.StartDate;
        entity.EndDate = request.EndDate;
        entity.UpdatedAt = dateTimeProvider.UtcNow;

        await banners.UpdateAsync(entity, cancellationToken);
        return entity.ToDto();
    }
}
