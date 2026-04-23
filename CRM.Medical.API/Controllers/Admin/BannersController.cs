using CRM.Medical.API.Contracts.Admin.Banners;
using CRM.Medical.API.Mapping;
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
public sealed class BannersController(ISender mediator) : AdminBaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<BannerDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(CancellationToken ct) =>
        Ok(await mediator.Send(new ListWebsiteBannersQuery(null), ct));

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(BannerDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromForm] CreateBannerRequest request, CancellationToken ct)
    {
        var visibility = JsonRequestParsing.ParseOptionalJsonElement(
            request.VisibilityRulesJson,
            "Invalid banner data",
            "Visibility rules must be valid JSON.");

        var command = new CreateBannerCommand(
            request.Title,
            request.Type,
            request.Media!,
            request.InternalLink ?? string.Empty,
            request.ExternalLink ?? string.Empty,
            request.TargetType,
            request.Location,
            request.DisplayOrder,
            request.IsActive,
            visibility,
            request.StartDate!.Value,
            request.EndDate!.Value);

        var dto = await mediator.Send(command, ct);
        return StatusCode(StatusCodes.Status201Created, dto);
    }
}
