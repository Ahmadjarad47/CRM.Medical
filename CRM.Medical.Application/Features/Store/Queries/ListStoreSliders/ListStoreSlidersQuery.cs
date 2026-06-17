using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Queries.ListStoreSliders;

public sealed record ListStoreSlidersQuery : IRequest<IReadOnlyList<StoreSliderDto>>;
