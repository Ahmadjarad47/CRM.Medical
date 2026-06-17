using CRM.Medical.Application.Features.Ads.DTOs;
using CRM.Medical.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CRM.Medical.Application.Features.Ads.Commands.UpdateAd;

public sealed record UpdateAdCommand(
    int Id,
    string Name,
    string Description,
    AdMediaType MediaType,
    IFormFile? Media) : IRequest<AdDto>;
