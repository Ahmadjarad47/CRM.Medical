using CRM.Medical.API.Contracts.Admin.Roles;
using CRM.Medical.API.Contracts.Common;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Roles.Commands.CreateRole;
using CRM.Medical.Application.Features.Roles.Commands.DeleteRole;
using CRM.Medical.Application.Features.Roles.Commands.UpdateRole;
using CRM.Medical.Application.Features.Roles.DTOs;
using CRM.Medical.Application.Features.Roles.Queries.GetRoleById;
using CRM.Medical.Application.Features.Roles.Queries.GetRoles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Admin;

[Route("api/admin/roles")]
public sealed class RolesController(ISender mediator) : AdminBaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<RoleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] PagedSearchRequest request, CancellationToken ct) =>
        Ok(await mediator.Send(new GetRolesQuery(request.Page, request.PageSize, request.Search), ct));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetRoleByIdQuery(id), ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequest request, CancellationToken ct)
    {
        var created = await mediator.Send(new CreateRoleCommand(request.Name), ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateRoleRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateRoleCommand(id, request.Name), ct);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        await mediator.Send(new DeleteRoleCommand(id), ct);
        return NoContent();
    }

}
