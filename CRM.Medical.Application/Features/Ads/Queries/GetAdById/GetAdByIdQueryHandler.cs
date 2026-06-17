using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Ads.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Ads.Queries.GetAdById;

public sealed class GetAdByIdQueryHandler(IAdRepository ads)
    : IRequestHandler<GetAdByIdQuery, AdDto>
{
    public async Task<AdDto> Handle(GetAdByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await ads.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Ad '{request.Id}' was not found.");

        return entity.ToDto();
    }
}
