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
        [FromForm] string title,
        [FromForm] string type,
        [FromForm] string? internalLink,
        [FromForm] string? externalLink,
        [FromForm] string targetType,
        [FromForm] string location,
        [FromForm] int displayOrder,
        [FromForm] bool isActive,
        [FromForm] string? visibilityRulesJson,
        [FromForm] DateTime startDate,
        [FromForm] DateTime endDate,
        [FromForm] IFormFile media,
        CancellationToken ct)
    {
        var command = await bannerCreateCommandFactory.CreateAsync(
            title,
            type,
            internalLink,
            externalLink,
            targetType,
            location,
            displayOrder,
            isActive,
            visibilityRulesJson,
            startDate,
            endDate,
            media,
            ct);

        var dto = await mediator.Send(command, ct);
        return StatusCode(StatusCodes.Status201Created, dto);
    }
}

