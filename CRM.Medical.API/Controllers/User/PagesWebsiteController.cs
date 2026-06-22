using CRM.Medical.Application.Features.Pages.DTOs;
using CRM.Medical.Application.Features.Pages.Queries.GetWebsitePageBySlug;
using CRM.Medical.Application.Features.Pages.Queries.ListWebsiteNavigationPages;
using Microsoft.AspNetCore.Authorization;

namespace CRM.Medical.API.Controllers.User;

/// <summary>Public dynamic pages for website rendering (no auth required).</summary>
[AllowAnonymous]
[Route("api/website/pages")]
public sealed class PagesWebsiteController(ISender mediator) : ControllerBase
{
    [HttpGet("navigation")]
    [ProducesResponseType(typeof(IReadOnlyList<WebsiteNavigationPageDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListNavigation([FromQuery] string language = "en-US", CancellationToken ct = default) =>
        Ok(await mediator.Send(new ListWebsiteNavigationPagesQuery(language), ct));

    [HttpGet("{slug}")]
    [ProducesResponseType(typeof(WebsitePageDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBySlug(string slug, [FromQuery] string language = "en-US", CancellationToken ct = default) =>
        Ok(await mediator.Send(new GetWebsitePageBySlugQuery(slug, language), ct));
}
