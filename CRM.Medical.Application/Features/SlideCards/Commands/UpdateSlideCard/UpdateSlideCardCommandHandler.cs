using CRM.Medical.Application.Common.Storage;
using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.SlideCards.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.SlideCards.Commands.UpdateSlideCard;

public sealed class UpdateSlideCardCommandHandler(
    ISlideCardRepository slideCards,
    IFileStorageService fileStorage,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<UpdateSlideCardCommand, SlideCardDto>
{
    public async Task<SlideCardDto> Handle(
        UpdateSlideCardCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await slideCards.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"SlideCard '{request.Id}' was not found.");

        if (request.Image is { Length: > 0 })
            entity.ImageUrl = await fileStorage.UploadImageAsync(request.Image, cancellationToken);

        entity.Title = request.Title.Trim();
        entity.Description = request.Description.Trim();
        entity.Price = request.Price;
        entity.Discount = request.Discount;
        entity.ExpiryDate = request.ExpiryDate;
        entity.Badge = request.Badge.Trim();
        entity.DetailPageLink = request.DetailPageLink.Trim();
        entity.DisplayOrder = request.DisplayOrder;
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = dateTimeProvider.UtcNow;

        await slideCards.UpdateAsync(entity, cancellationToken);
        return entity.ToDto();
    }
}
