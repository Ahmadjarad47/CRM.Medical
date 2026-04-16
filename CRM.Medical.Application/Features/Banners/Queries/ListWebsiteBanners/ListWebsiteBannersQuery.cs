using CRM.Medical.Application.Features.Banners.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Banners.Queries.ListWebsiteBanners;

public sealed record ListWebsiteBannersQuery(string? Location) : IRequest<IReadOnlyList<BannerDto>>;

