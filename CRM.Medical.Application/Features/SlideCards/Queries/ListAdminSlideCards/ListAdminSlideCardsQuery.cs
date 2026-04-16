using CRM.Medical.Application.Features.SlideCards.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.SlideCards.Queries.ListAdminSlideCards;

public sealed record ListAdminSlideCardsQuery : IRequest<IReadOnlyList<SlideCardDto>>;

