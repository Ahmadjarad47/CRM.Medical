using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.SlideCards.DTOs;
using CRM.Medical.Application.Features.SlideCards.Queries.ListWebsiteSlideCards;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.User;

/// <summary>Public slide cards for website display (no auth required).</summary>
[AllowAnonymous]
[Route("api/website/slide-cards")]
public sealed class SlideCardsWebsiteController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SlideCardDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(CancellationToken ct) =>
        Ok(await mediator.Send(new ListWebsiteSlideCardsQuery(), ct));
}

