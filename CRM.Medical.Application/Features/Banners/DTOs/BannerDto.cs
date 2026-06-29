using CRM.Medical.Domain.Enums;

namespace CRM.Medical.Application.Features.Banners.DTOs;

public sealed record BannerDto(
    int Id,
    string Title,
    DisplayMode DisplayMode,
    string MediaUrl,
    string InternalLink,
    string ExternalLink,
    string TargetType,
    string Location,
    int DisplayOrder,
    bool IsActive,
    object? VisibilityRules,
    DateTime StartDate,
    DateTime EndDate,
    int ViewsCount,
    int ClicksCount,
    DateTime CreatedAt);

