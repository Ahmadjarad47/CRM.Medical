using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.WelcomePages.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.WelcomePages.Queries.GetWelcomePageById;

public sealed class GetWelcomePageByIdQueryHandler(IWelcomePageRepository welcomePages)
    : IRequestHandler<GetWelcomePageByIdQuery, WelcomePageDto>
{
    public async Task<WelcomePageDto> Handle(GetWelcomePageByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await welcomePages.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Welcome page '{request.Id}' was not found.");

        return entity.ToDto();
    }
}
