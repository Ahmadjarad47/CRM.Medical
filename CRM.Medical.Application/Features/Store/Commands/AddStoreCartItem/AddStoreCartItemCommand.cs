using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.AddStoreCartItem;

public sealed record AddStoreCartItemCommand(string LabClientId, int ProductId, int Quantity) : IRequest<CartDto>;
