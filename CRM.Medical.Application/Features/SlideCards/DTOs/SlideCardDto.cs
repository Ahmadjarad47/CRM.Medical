namespace CRM.Medical.Application.Features.SlideCards.DTOs;

public sealed record SlideCardDto(
    int Id,
    string Title,
    string Description,
    string ImageUrl,
    decimal Price,
    decimal Discount,
    DateTime ExpiryDate,
    string Badge,
    string DetailPageLink,
    int DisplayOrder,
    bool IsActive,
    DateTime CreatedAt);

