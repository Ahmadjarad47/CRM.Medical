using CRM.Medical.Application.Features.WelcomePages.DTOs;
using CRM.Medical.Application.Features.WelcomePages.Queries.ListWebsiteWelcomePages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.User;

/// <summary>Public welcome page items for website display (no auth required).</summary>
[AllowAnonymous]
[Route("api/website/welcome-pages")]
public sealed class WelcomePagesWebsiteController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<WelcomePageDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(CancellationToken ct) =>
        Ok(await mediator.Send(new ListWebsiteWelcomePagesQuery(), ct));
}
