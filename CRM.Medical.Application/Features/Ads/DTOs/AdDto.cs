using CRM.Medical.Domain.Enums;

namespace CRM.Medical.Application.Features.Ads.DTOs;

public sealed record AdDto(
    int Id,
    string Name,
    string Description,
    AdMediaType MediaType,
    string MediaUrl,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
