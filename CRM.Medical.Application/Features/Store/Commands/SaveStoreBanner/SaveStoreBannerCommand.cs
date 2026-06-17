using CRM.Medical.Application.Features.Store.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.SaveStoreBanner;

public sealed record SaveStoreBannerCommand(
    int? Id,
    string Title,
    string ImageUrl,
    string? LinkUrl,
    string Location,
    int? CategoryId,
    int DisplayOrder,
    bool IsActive,
    DateTime? StartsAt,
    DateTime? EndsAt) : IRequest<StoreBannerDto>;
