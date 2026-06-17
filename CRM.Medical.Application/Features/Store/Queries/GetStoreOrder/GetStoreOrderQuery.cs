using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Queries.GetStoreOrder;

public sealed record GetStoreOrderQuery(int Id) : IRequest<StoreOrderDetailsDto>;
