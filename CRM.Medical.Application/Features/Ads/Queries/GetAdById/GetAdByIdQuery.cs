using CRM.Medical.Application.Features.Ads.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Ads.Queries.GetAdById;

public sealed record GetAdByIdQuery(int Id) : IRequest<AdDto>;
