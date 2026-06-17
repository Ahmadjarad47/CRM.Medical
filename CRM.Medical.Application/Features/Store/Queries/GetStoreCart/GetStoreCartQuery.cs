using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Queries.GetStoreCart;

public sealed record GetStoreCartQuery(string LabClientId) : IRequest<CartDto>;
