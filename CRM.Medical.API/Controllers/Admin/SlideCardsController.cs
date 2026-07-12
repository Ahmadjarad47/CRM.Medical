using CRM.Medical.API.Contracts.Admin.SlideCards;
using CRM.Medical.Application.Features.SlideCards.Commands.CreateSlideCard;
using CRM.Medical.Application.Features.SlideCards.Commands.UpdateSlideCard;
using CRM.Medical.Application.Features.SlideCards.DTOs;
using CRM.Medical.Application.Features.SlideCards.Queries.GetSlideCardById;
using CRM.Medical.Application.Features.SlideCards.Queries.ListAdminSlideCards;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Admin;

[Route("api/admin/slide-cards")]
public sealed class SlideCardsController(ISender mediator) : AdminBaseController
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
    public async Task<IActionResult> Create([FromForm] CreateSlideCardRequest request, CancellationToken ct)
    {
        var command = new CreateSlideCardCommand(
            request.Title,
            request.Description,
            request.Image!,
            request.Price,
            request.Discount,
            request.ExpiryDate!.Value,
            request.Badge,
            request.DetailPageLink,
            request.DisplayOrder,
            request.IsActive);

        var dto = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:int}")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(SlideCardDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(int id, [FromForm] UpdateSlideCardRequest request, CancellationToken ct)
    {
        var command = new UpdateSlideCardCommand(
            id,
            request.Title,
            request.Description,
            request.Image,
            request.Price,
            request.Discount,
            request.ExpiryDate!.Value,
            request.Badge,
            request.DetailPageLink,
            request.DisplayOrder,
            request.IsActive);

        return Ok(await mediator.Send(command, ct));
    }
}
