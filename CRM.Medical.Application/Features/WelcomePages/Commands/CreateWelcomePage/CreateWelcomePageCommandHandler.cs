using CRM.Medical.Application.Common.Storage;
using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Features.WelcomePages.DTOs;
using CRM.Medical.Domain.Entities;
using MediatR;

namespace CRM.Medical.Application.Features.WelcomePages.Commands.CreateWelcomePage;

public sealed class CreateWelcomePageCommandHandler(
    IWelcomePageRepository welcomePages,
    IFileStorageService fileStorage,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<CreateWelcomePageCommand, WelcomePageDto>
{
    public async Task<WelcomePageDto> Handle(CreateWelcomePageCommand request, CancellationToken cancellationToken)
    {
        var mediaUrl = await fileStorage.UploadFileAsync(request.Media, "welcome-pages", cancellationToken);

        var entity = new WelcomePage
        {
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            MediaType = request.MediaType,
            MediaUrl = mediaUrl,
            IsActive = request.IsActive,
            CreatedAt = dateTimeProvider.UtcNow
        };

        await welcomePages.AddAsync(entity, cancellationToken);
        return entity.ToDto();
    }
}
