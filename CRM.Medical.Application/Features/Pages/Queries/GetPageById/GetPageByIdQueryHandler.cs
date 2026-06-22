using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Pages.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Pages.Queries.GetPageById;

public sealed class GetPageByIdQueryHandler(IPageRepository pages)
    : IRequestHandler<GetPageByIdQuery, PageDto>
{
    public async Task<PageDto> Handle(GetPageByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await pages.GetByIdWithDetailsAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Page '{request.Id}' was not found.");

        return entity.ToDto();
    }
}
