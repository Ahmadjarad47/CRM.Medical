using CRM.Medical.API.Controllers.Admin.Models;
using CRM.Medical.Application.Features.Permissions.Commands.CreatePermission;
using CRM.Medical.Application.Features.Permissions.Commands.DeletePermission;
using CRM.Medical.Application.Features.Permissions.Commands.UpdatePermission;
using CRM.Medical.Application.Features.Permissions.Queries.GetPermissionById;
using CRM.Medical.Application.Features.Permissions.Queries.GetPermissions;
using CRM.Medical.Application.Features.Users.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Admin;

[Route("api/admin/permissions")]
public sealed class PermissionsController(ISender mediator) : AdminBaseController
{
    [HttpGet]
    [Authorize(Policy = UserPermissions.PermissionsManage)]
    public async Task<IActionResult> List(CancellationToken ct) =>
        Ok(await mediator.Send(new GetPermissionsQuery(), ct));

    [HttpGet("{id:guid}")]
    [Authorize(Policy = UserPermissions.PermissionsManage)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetPermissionByIdQuery(id), ct));

    [HttpPost]
    [Authorize(Policy = UserPermissions.PermissionsManage)]
    public async Task<IActionResult> Create([FromBody] CreatePermissionRequest request, CancellationToken ct)
    {
        var created = await mediator.Send(new CreatePermissionCommand(request.Name, request.Description), ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = UserPermissions.PermissionsManage)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePermissionRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdatePermissionCommand(id, request.Description), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = UserPermissions.PermissionsManage)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeletePermissionCommand(id), ct);
        return NoContent();
    }
}
