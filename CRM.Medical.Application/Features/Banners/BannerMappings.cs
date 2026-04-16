using System.Text.Json;
using CRM.Medical.Application.Features.Banners.DTOs;
using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Features.Banners;

internal static class BannerMappings
{
    public static BannerDto ToDto(this Banner b) =>
        new(
            b.Id,
            b.Title,
            b.Type,
            b.MediaUrl,
            b.InternalLink,
            b.ExternalLink,
            b.TargetType,
            b.Location,
            b.DisplayOrder,
            b.IsActive,
            b.VisibilityRules is null ? null : JsonSerializer.Deserialize<object>(b.VisibilityRules.RootElement.GetRawText()),
            b.StartDate,
            b.EndDate,
            b.ViewsCount,
            b.ClicksCount,
            b.CreatedAt);
}

