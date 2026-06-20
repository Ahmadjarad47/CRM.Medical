using CRM.Medical.Application.Common.Storage;
using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.WelcomePages.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.WelcomePages.Commands.UpdateWelcomePage;

public sealed class UpdateWelcomePageCommandHandler(
    IWelcomePageRepository welcomePages,
    IFileStorageService fileStorage,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<UpdateWelcomePageCommand, WelcomePageDto>
{
    public async Task<WelcomePageDto> Handle(UpdateWelcomePageCommand request, CancellationToken cancellationToken)
    {
        var entity = await welcomePages.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Welcome page '{request.Id}' was not found.");

        if (request.Media is { Length: > 0 })
            entity.MediaUrl = await fileStorage.UploadFileAsync(request.Media, "welcome-pages", cancellationToken);

        entity.Name = request.Name.Trim();
        entity.Description = request.Description.Trim();
        entity.MediaType = request.MediaType;
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = dateTimeProvider.UtcNow;

        await welcomePages.UpdateAsync(entity, cancellationToken);
        return entity.ToDto();
    }
}
