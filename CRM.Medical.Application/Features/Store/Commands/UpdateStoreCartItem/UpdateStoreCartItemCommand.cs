using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.UpdateStoreCartItem;

public sealed record UpdateStoreCartItemCommand(string LabClientId, int ItemId, int Quantity) : IRequest<CartDto>;
