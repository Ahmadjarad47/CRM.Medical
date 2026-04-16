using CRM.Medical.Application.Features.Banners.DTOs;
using CRM.Medical.Application.Features.Banners.Queries.ListWebsiteBanners;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.User;

/// <summary>Public banners for website display (no auth required).</summary>
[AllowAnonymous]
[Route("api/website/banners")]
public sealed class BannersWebsiteController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<BannerDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] string? location, CancellationToken ct) =>
        Ok(await mediator.Send(new ListWebsiteBannersQuery(location), ct));
}

