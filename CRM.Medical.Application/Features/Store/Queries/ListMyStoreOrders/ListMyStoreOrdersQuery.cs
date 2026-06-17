using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Queries.ListMyStoreOrders;

public sealed record ListMyStoreOrdersQuery(string LabClientId, int Page, int PageSize) : IRequest<PagedResult<StoreOrderDto>>;
