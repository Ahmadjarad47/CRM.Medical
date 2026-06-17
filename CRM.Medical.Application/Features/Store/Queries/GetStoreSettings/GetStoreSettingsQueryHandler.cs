using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Queries.GetStoreSettings;

public sealed class GetStoreSettingsQueryHandler(IStoreAdminService service)
    : IRequestHandler<GetStoreSettingsQuery, StoreSettingDto>
{
    public Task<StoreSettingDto> Handle(GetStoreSettingsQuery request, CancellationToken cancellationToken) =>
        service.GetSettingsAsync(cancellationToken);
}
