using CRM.Medical.API.Contracts.Admin.WelcomePages;
using CRM.Medical.Application.Features.WelcomePages.Commands.CreateWelcomePage;
using CRM.Medical.Application.Features.WelcomePages.Commands.DeleteWelcomePage;
using CRM.Medical.Application.Features.WelcomePages.Commands.UpdateWelcomePage;
using CRM.Medical.Application.Features.WelcomePages.DTOs;
using CRM.Medical.Application.Features.WelcomePages.Queries.GetWelcomePageById;
using CRM.Medical.Application.Features.WelcomePages.Queries.ListWelcomePages;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Admin;

[Route("api/admin/welcome-pages")]
public sealed class WelcomePagesController(ISender mediator) : AdminBaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<WelcomePageDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(CancellationToken ct) =>
        Ok(await mediator.Send(new ListWelcomePagesQuery(), ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(WelcomePageDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetWelcomePageByIdQuery(id), ct));

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(WelcomePageDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromForm] CreateWelcomePageRequest request, CancellationToken ct)
    {
        var command = new CreateWelcomePageCommand(
            request.Name,
            request.Description,
            request.MediaType,
            request.Media!,
            request.IsActive);

        var dto = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPatch("{id:int}")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(WelcomePageDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(int id, [FromForm] UpdateWelcomePageRequest request, CancellationToken ct)
    {
        var command = new UpdateWelcomePageCommand(
            id,
            request.Name,
            request.Description,
            request.MediaType,
            request.Media,
            request.IsActive);

        return Ok(await mediator.Send(command, ct));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await mediator.Send(new DeleteWelcomePageCommand(id), ct);
        return NoContent();
    }
}
