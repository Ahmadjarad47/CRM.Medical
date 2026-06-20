using CRM.Medical.Application.Features.Ads.DTOs;
using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Features.Ads;

internal static class AdMappings
{
    public static AdDto ToDto(this Ad ad) =>
        new(
            ad.Id,
            ad.Name,
            ad.Description,
            ad.MediaType,
            ad.MediaUrl,
            ad.Latitude,
            ad.Longitude,
            ad.AddressName,
            ad.CreatedAt,
            ad.UpdatedAt);
}
