using CRM.Medical.Domain.Enums;

namespace CRM.Medical.Application.Features.Ads.DTOs;

public sealed record AdDto(
    int Id,
    string Name,
    string Description,
    AdMediaType MediaType,
    DisplayMode DisplayMode,
    string MediaUrl,
    double? Latitude,
    double? Longitude,
    string AddressName,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
