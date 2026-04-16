using System.IO;
using CRM.Medical.Application.Features.SlideCards.Commands.CreateSlideCard;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CRM.Medical.API.Services.SlideCards;

public interface ISlideCardCreateCommandFactory
{
    Task<CreateSlideCardCommand> CreateAsync(
        string title,
        string description,
        decimal price,
        decimal discount,
        DateTime expiryDate,
        string badge,
        string detailPageLink,
        int displayOrder,
        bool isActive,
        IFormFile? image,
        CancellationToken cancellationToken);
}

public sealed class SlideCardCreateCommandFactory : ISlideCardCreateCommandFactory
{
    public async Task<CreateSlideCardCommand> CreateAsync(
        string title,
        string description,
        decimal price,
        decimal discount,
        DateTime expiryDate,
        string badge,
        string detailPageLink,
        int displayOrder,
        bool isActive,
        IFormFile? image,
        CancellationToken cancellationToken)
    {
        byte[] fileBytes = [];
        var contentType = string.Empty;
        var fileName = string.Empty;

        if (image is { Length: > 0 })
        {
            await using var ms = new MemoryStream();
            await image.CopyToAsync(ms, cancellationToken);
            fileBytes = ms.ToArray();
            contentType = image.ContentType;
            fileName = image.FileName;
        }

        return new CreateSlideCardCommand(
            title,
            description,
            fileBytes,
            contentType,
            fileName,
            price,
            discount,
            expiryDate,
            badge,
            detailPageLink,
            displayOrder,
            isActive);
    }
}

