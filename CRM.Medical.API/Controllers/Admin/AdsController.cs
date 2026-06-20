using CRM.Medical.API.Contracts.Admin.Ads;
using CRM.Medical.Application.Features.Ads.Commands.CreateAd;
using CRM.Medical.Application.Features.Ads.Commands.DeleteAd;
using CRM.Medical.Application.Features.Ads.Commands.UpdateAd;
using CRM.Medical.Application.Features.Ads.DTOs;
using CRM.Medical.Application.Features.Ads.Queries.GetAdById;
using CRM.Medical.Application.Features.Ads.Queries.ListAds;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Admin;

[Route("api/admin/ads")]
public sealed class AdsController(ISender mediator) : AdminBaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AdDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(CancellationToken ct) =>
        Ok(await mediator.Send(new ListAdsQuery(), ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AdDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetAdByIdQuery(id), ct));

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(AdDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(
        [FromForm] CreateAdRequest request,
        CancellationToken ct)
    {
        var command = new CreateAdCommand(
            request.Name,
            request.Description,
            request.Latitude,
            request.Longitude,
            request.AddressName,
            request.MediaType,
            request.Media!
        );

        var dto = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPatch("{id:int}")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(AdDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(
        int id,
        [FromForm] UpdateAdRequest request,
        CancellationToken ct)
    {
        var command = new UpdateAdCommand(
            id,
            request.Name,
            request.Description,
            request.Latitude,
            request.Longitude,
            request.AddressName,
            request.MediaType,
            request.Media
        );

        return Ok(await mediator.Send(command, ct));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await mediator.Send(new DeleteAdCommand(id), ct);
        return NoContent();
    }
}
