using CRM.Medical.Application.Features.SlideCards.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.SlideCards.Queries.GetSlideCardById;

public sealed record GetSlideCardByIdQuery(int Id) : IRequest<SlideCardDto>;

