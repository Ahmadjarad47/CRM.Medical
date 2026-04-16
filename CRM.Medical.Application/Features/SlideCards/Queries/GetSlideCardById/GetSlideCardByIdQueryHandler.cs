using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.SlideCards.DTOs;
using CRM.Medical.Application.Features.SlideCards;
using CRM.Medical.Domain.Entities;
using MediatR;

namespace CRM.Medical.Application.Features.SlideCards.Queries.GetSlideCardById;

public sealed class GetSlideCardByIdQueryHandler(
    ISlideCardRepository repository)
    : IRequestHandler<GetSlideCardByIdQuery, SlideCardDto>
{
    public async Task<SlideCardDto> Handle(
        GetSlideCardByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"SlideCard '{request.Id}' was not found.");

        return entity.ToDto();
    }
}

