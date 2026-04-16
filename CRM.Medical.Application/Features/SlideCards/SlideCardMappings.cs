using CRM.Medical.Application.Features.SlideCards.DTOs;
using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Features.SlideCards;

internal static class SlideCardMappings
{
    public static SlideCardDto ToDto(this SlideCard s) =>
        new(
            s.Id,
            s.Title,
            s.Description,
            s.ImageUrl,
            s.Price,
            s.Discount,
            s.ExpiryDate,
            s.Badge,
            s.DetailPageLink,
            s.DisplayOrder,
            s.IsActive,
            s.CreatedAt);
}

