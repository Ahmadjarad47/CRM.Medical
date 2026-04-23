using CRM.Medical.API.Contracts.User.Templates;
using CRM.Medical.API.Extensions;
using CRM.Medical.Application.Features.Templates.Commands.CreateTemplate;
using CRM.Medical.Application.Features.Templates.DTOs;
using CRM.Medical.Application.Features.Templates.Queries.GetMyTemplateById;
using CRM.Medical.Application.Features.Templates.Queries.ListMyTemplates;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.User;

[Route("api/users/me/templates")]
public sealed class MeTemplatesController(ISender mediator) : UserBaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TemplateDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(CancellationToken ct) =>
        Ok(await mediator.Send(new ListMyTemplatesQuery(User.GetRequiredRole()), ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TemplateDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetMyTemplateByIdQuery(User.GetRequiredRole(), id), ct));

    [HttpPost]
    [ProducesResponseType(typeof(TemplateDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateTemplateRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(
            new CreateTemplateCommand(
                request.Name,
                request.Data,
                User.GetRequiredRole()),
            ct);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}
