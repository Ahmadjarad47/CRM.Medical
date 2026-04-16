using CRM.Medical.Application.Features.SlideCards.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.SlideCards.Commands.CreateSlideCard;

public sealed record CreateSlideCardCommand(
    string Title,
    string Description,
    byte[] ImageBytes,
    string ImageContentType,
    string ImageFileName,
    decimal Price,
    decimal Discount,
    DateTime ExpiryDate,
    string Badge,
    string DetailPageLink,
    int DisplayOrder,
    bool IsActive) : IRequest<SlideCardDto>;

