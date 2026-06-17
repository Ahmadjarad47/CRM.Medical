using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Queries.GetStoreHome;

public sealed class GetStoreHomeQueryHandler(IStoreHomeService service)
    : IRequestHandler<GetStoreHomeQuery, StoreHomeDto>
{
    public Task<StoreHomeDto> Handle(GetStoreHomeQuery request, CancellationToken cancellationToken) =>
        service.GetHomeAsync(cancellationToken);
}
