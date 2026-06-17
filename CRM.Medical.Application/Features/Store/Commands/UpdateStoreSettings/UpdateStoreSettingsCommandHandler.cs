using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.UpdateStoreSettings;

public sealed class UpdateStoreSettingsCommandHandler(IStoreAdminService service)
    : IRequestHandler<UpdateStoreSettingsCommand, StoreSettingDto>
{
    public Task<StoreSettingDto> Handle(UpdateStoreSettingsCommand request, CancellationToken cancellationToken) =>
        service.UpdateSettingsAsync(request.Request, cancellationToken);
}
