using CRM.Medical.API.Services.Banners;
using CRM.Medical.Application.Features.Banners.Commands.CreateBanner;
using CRM.Medical.Application.Features.Banners.DTOs;
using CRM.Medical.Application.Features.Banners.Queries.ListWebsiteBanners;
using CRM.Medical.Application.Features.Users.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Admin;

public sealed class CreateBannerRequest
{
    public string Title { get; init; } = default!;
    public string Type { get; init; } = default!;
    public string? InternalLink { get; init; }
    public string? ExternalLink { get; init; }
    public string TargetType { get; init; } = default!;
    public string Location { get; init; } = default!;
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
    public string? VisibilityRulesJson { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public IFormFile Media { get; init; } = default!;
}

[Route("api/admin/banners")]
[Authorize(Roles = UserRoles.Admin)]
public sealed class BannersController(
    ISender mediator,
    IBannerCreateCommandFactory bannerCreateCommandFactory) : AdminBaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<BannerDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(CancellationToken ct) =>
        Ok(await mediator.Send(new ListWebsiteBannersQuery(null), ct));

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(BannerDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(
        [FromForm] CreateBannerRequest request,
        CancellationToken ct)
    {
        var command = await bannerCreateCommandFactory.CreateAsync(
            request.Title,
            request.Type,
            request.InternalLink,
            request.ExternalLink,
            request.TargetType,
            request.Location,
            request.DisplayOrder,
            request.IsActive,
            request.VisibilityRulesJson,
            request.StartDate,
            request.EndDate,
            request.Media,
            ct);

        var dto = await mediator.Send(command, ct);
        return StatusCode(StatusCodes.Status201Created, dto);
    }
}

