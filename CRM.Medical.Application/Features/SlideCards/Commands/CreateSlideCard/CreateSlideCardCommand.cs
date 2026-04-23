using CRM.Medical.Application.Features.SlideCards.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CRM.Medical.Application.Features.SlideCards.Commands.CreateSlideCard;

public sealed record CreateSlideCardCommand(
    string Title,
    string Description,
    IFormFile Image,
    decimal Price,
    decimal Discount,
    DateTime ExpiryDate,
    string Badge,
    string DetailPageLink,
    int DisplayOrder,
    bool IsActive) : IRequest<SlideCardDto>;

