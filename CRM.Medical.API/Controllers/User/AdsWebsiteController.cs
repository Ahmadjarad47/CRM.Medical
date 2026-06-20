using CRM.Medical.Application.Features.Ads.DTOs;
using CRM.Medical.Application.Features.Ads.Queries.ListAds;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.User;

/// <summary>Public ads for website display (no auth required).</summary>
[AllowAnonymous]
[Route("api/website/ads")]
public sealed class AdsWebsiteController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AdDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(CancellationToken ct) =>
        Ok(await mediator.Send(new ListAdsQuery(), ct));
}

