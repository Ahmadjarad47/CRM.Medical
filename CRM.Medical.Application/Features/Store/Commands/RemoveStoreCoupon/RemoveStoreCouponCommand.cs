using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.RemoveStoreCoupon;

public sealed record RemoveStoreCouponCommand(string LabClientId) : IRequest<CartDto>;
