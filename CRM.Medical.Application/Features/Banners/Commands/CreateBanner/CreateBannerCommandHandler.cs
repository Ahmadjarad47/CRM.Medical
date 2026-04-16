using System.IO;
using System.Text.Json;
using CRM.Medical.Application.Common.Storage;
using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Features.Banners.DTOs;
using CRM.Medical.Domain.Entities;
using MediatR;

namespace CRM.Medical.Application.Features.Banners.Commands.CreateBanner;

public sealed class CreateBannerCommandHandler(
    IBannerRepository banners,
    IObjectStorageService objectStorage,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<CreateBannerCommand, BannerDto>
{
    public async Task<BannerDto> Handle(
        CreateBannerCommand request,
        CancellationToken cancellationToken)
    {
        await using var stream = new MemoryStream(request.MediaBytes);
        var mediaUrl = await objectStorage.UploadAsync(
            stream,
            request.MediaContentType,
            request.MediaFileName,
            cancellationToken);

        JsonDocument? visibility = null;
        if (request.VisibilityRules is { } json)
        {
            visibility = JsonDocument.Parse(json.GetRawText());
        }

        var now = dateTimeProvider.UtcNow;
        var entity = new Banner
        {
            Title = request.Title.Trim(),
            Type = request.Type.Trim(),
            MediaUrl = mediaUrl,
            InternalLink = request.InternalLink?.Trim() ?? string.Empty,
            ExternalLink = request.ExternalLink?.Trim() ?? string.Empty,
            TargetType = request.TargetType.Trim(),
            Location = request.Location.Trim(),
            DisplayOrder = request.DisplayOrder,
            IsActive = request.IsActive,
            VisibilityRules = visibility,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            ViewsCount = 0,
            ClicksCount = 0,
            CreatedAt = now
        };

        await banners.AddAsync(entity, cancellationToken);
        return entity.ToDto();
    }
}

