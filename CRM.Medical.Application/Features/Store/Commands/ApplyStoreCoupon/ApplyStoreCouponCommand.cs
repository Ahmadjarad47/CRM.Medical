using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.ApplyStoreCoupon;

public sealed record ApplyStoreCouponCommand(string LabClientId, string Code) : IRequest<CartDto>;
