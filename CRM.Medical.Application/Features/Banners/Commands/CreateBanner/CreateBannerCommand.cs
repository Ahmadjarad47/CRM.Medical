using System.Text.Json;
using CRM.Medical.Application.Features.Banners.DTOs;
using CRM.Medical.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CRM.Medical.Application.Features.Banners.Commands.CreateBanner;

public sealed record CreateBannerCommand(
    string Title,
    DisplayMode DisplayMode,
    IFormFile Media,
    string InternalLink,
    string ExternalLink,
    string TargetType,
    string Location,
    int DisplayOrder,
    bool IsActive,
    JsonElement? VisibilityRules,
    DateTime StartDate,
    DateTime EndDate) : IRequest<BannerDto>;

