using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Domain.Enums;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.SaveStoreSlider;

public sealed record SaveStoreSliderCommand(
    int? Id,
    string Title,
    StoreSliderType Type,
    int DisplayOrder,
    bool IsActive,
    IReadOnlyList<int> ProductIds) : IRequest<StoreSliderDto>;
