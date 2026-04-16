using CRM.Medical.API.Services.SlideCards;
using CRM.Medical.Application.Features.SlideCards.Commands.CreateSlideCard;
using CRM.Medical.Application.Features.SlideCards.DTOs;
using CRM.Medical.Application.Features.SlideCards.Queries.GetSlideCardById;
using CRM.Medical.Application.Features.SlideCards.Queries.ListAdminSlideCards;
using CRM.Medical.Application.Features.Users.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Admin;

[Route("api/admin/slide-cards")]
[Authorize(Roles = UserRoles.Admin)]
public sealed class SlideCardsController(
    ISender mediator,
    ISlideCardCreateCommandFactory slideCardCreateCommandFactory) : AdminBaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SlideCardDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(CancellationToken ct) =>
        Ok(await mediator.Send(new ListAdminSlideCardsQuery(), ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(SlideCardDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetSlideCardByIdQuery(id), ct));

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(SlideCardDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(
        [FromForm] string title,
        [FromForm] string description,
        [FromForm] decimal price,
        [FromForm] decimal discount,
        [FromForm] DateTime expiryDate,
        [FromForm] string badge,
        [FromForm] string detailPageLink,
        [FromForm] int displayOrder,
        [FromForm] bool isActive,
        [FromForm] IFormFile? image,
        CancellationToken ct)
    {
        var command = await slideCardCreateCommandFactory.CreateAsync(
            title,
            description,
            price,
            discount,
            expiryDate,
            badge,
            detailPageLink,
            displayOrder,
            isActive,
            image,
            ct);

        var dto = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }
}

