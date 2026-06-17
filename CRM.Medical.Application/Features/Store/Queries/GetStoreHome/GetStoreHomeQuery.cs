using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Queries.GetStoreHome;

public sealed record GetStoreHomeQuery : IRequest<StoreHomeDto>;
