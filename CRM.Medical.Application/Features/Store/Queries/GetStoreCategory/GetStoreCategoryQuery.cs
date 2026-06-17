using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Queries.GetStoreCategory;

public sealed record GetStoreCategoryQuery(int Id, bool ActiveOnly) : IRequest<ProductCategoryDto>;
