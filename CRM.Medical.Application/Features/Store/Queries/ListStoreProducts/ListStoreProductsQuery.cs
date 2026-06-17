using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Queries.ListStoreProducts;

public sealed record ListStoreProductsQuery(int Page, int PageSize, string? Search, int? CategoryId, bool ActiveOnly) : IRequest<PagedResult<ProductCardDto>>;
