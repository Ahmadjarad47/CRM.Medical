using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Domain.Enums;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Queries.ListStoreOrders;

public sealed record ListStoreOrdersQuery(int Page, int PageSize, string? Search, StoreOrderStatus? Status) : IRequest<PagedResult<StoreOrderDto>>;
