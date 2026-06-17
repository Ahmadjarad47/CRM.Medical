using CRM.Medical.Application.Features.Ads.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Ads.Queries.ListAds;

public sealed record ListAdsQuery : IRequest<IReadOnlyList<AdDto>>;
