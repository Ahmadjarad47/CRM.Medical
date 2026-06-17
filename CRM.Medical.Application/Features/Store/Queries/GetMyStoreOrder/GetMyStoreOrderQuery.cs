using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Queries.GetMyStoreOrder;

public sealed record GetMyStoreOrderQuery(string LabClientId, int Id) : IRequest<StoreOrderDetailsDto>;
