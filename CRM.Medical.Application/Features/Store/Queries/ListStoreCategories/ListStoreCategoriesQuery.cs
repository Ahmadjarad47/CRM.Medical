using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Queries.ListStoreCategories;

public sealed record ListStoreCategoriesQuery(bool ActiveOnly) : IRequest<IReadOnlyList<ProductCategoryDto>>;
