using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Queries.GetStoreProduct;

public sealed record GetStoreProductQuery(int Id, bool ActiveOnly) : IRequest<ProductDetailsDto>;
