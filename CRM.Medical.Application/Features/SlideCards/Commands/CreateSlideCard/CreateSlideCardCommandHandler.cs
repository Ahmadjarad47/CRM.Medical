using System.IO;
using CRM.Medical.Application.Common.Storage;
using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Features.SlideCards.DTOs;
using CRM.Medical.Application.Features.SlideCards;
using CRM.Medical.Domain.Entities;
using MediatR;

namespace CRM.Medical.Application.Features.SlideCards.Commands.CreateSlideCard;

public sealed class CreateSlideCardCommandHandler(
    ISlideCardRepository slideCards,
    IObjectStorageService objectStorage,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<CreateSlideCardCommand, SlideCardDto>
{
    public async Task<SlideCardDto> Handle(
        CreateSlideCardCommand request,
        CancellationToken cancellationToken)
    {
        await using var stream = new MemoryStream(request.ImageBytes);
        var imageUrl = await objectStorage.UploadAsync(
            stream,
            request.ImageContentType,
            request.ImageFileName,
            cancellationToken);

        var now = dateTimeProvider.UtcNow;
        var entity = new SlideCard
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            ImageUrl = imageUrl,
            Price = request.Price,
            Discount = request.Discount,
            ExpiryDate = request.ExpiryDate,
            Badge = request.Badge.Trim(),
            DetailPageLink = request.DetailPageLink.Trim(),
            DisplayOrder = request.DisplayOrder,
            IsActive = request.IsActive,
            CreatedAt = now
        };

        await slideCards.AddAsync(entity, cancellationToken);
        return entity.ToDto();
    }
}

