using System.IO;
using System.Text.Json;
using CRM.Medical.Application.Features.Banners.Commands.CreateBanner;
using Microsoft.AspNetCore.Http;

namespace CRM.Medical.API.Services.Banners;

public interface IBannerCreateCommandFactory
{
    Task<CreateBannerCommand> CreateAsync(
        string title,
        string type,
        string? internalLink,
        string? externalLink,
        string targetType,
        string location,
        int displayOrder,
        bool isActive,
        string? visibilityRulesJson,
        DateTime startDate,
        DateTime endDate,
        IFormFile media,
        CancellationToken cancellationToken);
}

public sealed class BannerCreateCommandFactory : IBannerCreateCommandFactory
{
    public async Task<CreateBannerCommand> CreateAsync(
        string title,
        string type,
        string? internalLink,
        string? externalLink,
        string targetType,
        string location,
        int displayOrder,
        bool isActive,
        string? visibilityRulesJson,
        DateTime startDate,
        DateTime endDate,
        IFormFile media,
        CancellationToken cancellationToken)
    {
        if (media is not { Length: > 0 })
            throw new ArgumentException("Media file is required.", nameof(media));

        await using var ms = new MemoryStream();
        await media.CopyToAsync(ms, cancellationToken);

        JsonElement? visibility = null;
        if (!string.IsNullOrWhiteSpace(visibilityRulesJson))
        {
            var doc = JsonDocument.Parse(visibilityRulesJson);
            visibility = doc.RootElement.Clone();
        }

        return new CreateBannerCommand(
            title,
            type,
            ms.ToArray(),
            media.ContentType,
            media.FileName,
            internalLink ?? string.Empty,
            externalLink ?? string.Empty,
            targetType,
            location,
            displayOrder,
            isActive,
            visibility,
            startDate,
            endDate);
    }
}

