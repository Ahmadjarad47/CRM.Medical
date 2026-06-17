using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Domain.Enums;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.UpdateStoreOrderStatus;

public sealed record UpdateStoreOrderStatusCommand(int Id, StoreOrderStatus Status) : IRequest<StoreOrderDetailsDto>;
