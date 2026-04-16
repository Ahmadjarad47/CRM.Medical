using System.Text.Json;
using CRM.Medical.Application.Features.Banners.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Banners.Commands.CreateBanner;

public sealed record CreateBannerCommand(
    string Title,
    string Type,
    byte[] MediaBytes,
    string MediaContentType,
    string MediaFileName,
    string InternalLink,
    string ExternalLink,
    string TargetType,
    string Location,
    int DisplayOrder,
    bool IsActive,
    JsonElement? VisibilityRules,
    DateTime StartDate,
    DateTime EndDate) : IRequest<BannerDto>;

