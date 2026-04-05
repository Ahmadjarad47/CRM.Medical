using CRM.Medical.API.Controllers.Admin.Models;
using CRM.Medical.Application.Features.Roles.Commands.CreateRole;
using CRM.Medical.Application.Features.Roles.Commands.DeleteRole;
using CRM.Medical.Application.Features.Roles.Commands.UpdateRole;
using CRM.Medical.Application.Features.Roles.Queries.GetRoleById;
using CRM.Medical.Application.Features.Roles.Queries.GetRoles;
using CRM.Medical.Application.Features.Users.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Admin;

[Route("api/admin/roles")]
public sealed class RolesController(ISender mediator) : AdminBaseController
{
    [HttpGet]
    [Authorize(Policy = UserPermissions.RolesManage)]
    public async Task<IActionResult> List(CancellationToken ct) =>
        Ok(await mediator.Send(new GetRolesQuery(), ct));

    [HttpGet("{id}")]
    [Authorize(Policy = UserPermissions.RolesManage)]
    public async Task<IActionResult> GetById(string id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetRoleByIdQuery(id), ct));

    [HttpPost]
    [Authorize(Policy = UserPermissions.RolesManage)]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequest request, CancellationToken ct)
    {
        var created = await mediator.Send(new CreateRoleCommand(request.Name), ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = UserPermissions.RolesManage)]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateRoleRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateRoleCommand(id, request.Name), ct);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = UserPermissions.RolesManage)]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        await mediator.Send(new DeleteRoleCommand(id), ct);
        return NoContent();
    }
}
