using CRM.Medical.Application.Features.SlideCards.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.SlideCards.Queries.ListWebsiteSlideCards;

public sealed record ListWebsiteSlideCardsQuery : IRequest<IReadOnlyList<SlideCardDto>>;

