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

public sealed class CreateSlideCardRequest
{
    public string Title { get; init; } = default!;
    public string Description { get; init; } = default!;
    public decimal Price { get; init; }
    public decimal Discount { get; init; }
    public DateTime ExpiryDate { get; init; }
    public string Badge { get; init; } = default!;
    public string DetailPageLink { get; init; } = default!;
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
    public IFormFile? Image { get; init; }
}

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
        [FromForm] CreateSlideCardRequest request,
        CancellationToken ct)
    {
        var command = await slideCardCreateCommandFactory.CreateAsync(
            request.Title,
            request.Description,
            request.Price,
            request.Discount,
            request.ExpiryDate,
            request.Badge,
            request.DetailPageLink,
            request.DisplayOrder,
            request.IsActive,
            request.Image,
            ct);

        var dto = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }
}

