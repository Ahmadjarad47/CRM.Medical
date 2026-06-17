using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Queries.ListStoreBanners;

public sealed record ListStoreBannersQuery : IRequest<IReadOnlyList<StoreBannerDto>>;
