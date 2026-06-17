using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Domain.Enums;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.StoreCheckout;

public sealed record StoreCheckoutCommand(string LabClientId, PaymentMethod PaymentMethod, string? Notes) : IRequest<StoreOrderDetailsDto>;
