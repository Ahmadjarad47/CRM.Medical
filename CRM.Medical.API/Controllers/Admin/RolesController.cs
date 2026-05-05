using CRM.Medical.API.Contracts.Admin.Roles;
using CRM.Medical.API.Contracts.Common;
using CRM.Medical.API.Contracts.Permissions;
using CRM.Medical.API.Authorization;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Permissions.CQRS;
using CRM.Medical.Application.Features.Permissions.DTOs;
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
    [DynamicAuthorize("Role", "Manage")]
    [ProducesResponseType(typeof(PagedResult<RoleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] PagedSearchRequest request, CancellationToken ct) =>
        Ok(await mediator.Send(new GetRolesQuery(request.Page, request.PageSize, request.Search), ct));

    [HttpGet("{id}")]
    [DynamicAuthorize("Role", "Manage")]
    public async Task<IActionResult> GetById(string id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetRoleByIdQuery(id), ct));

    [HttpPost]
    [DynamicAuthorize("Role", "Manage")]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequest request, CancellationToken ct)
    {
        var created = await mediator.Send(new CreateRoleCommand(request.Name), ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [DynamicAuthorize("Role", "Manage")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateRoleRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateRoleCommand(id, request.Name), ct);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [DynamicAuthorize("Role", "Manage")]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        await mediator.Send(new DeleteRoleCommand(id), ct);
        return NoContent();
    }

    [HttpGet("{id}/permissions")]
    [DynamicAuthorize("Role", "Manage")]
    [ProducesResponseType(typeof(IReadOnlyList<AccessPolicyDto>), StatusCodes.Status200OK)]
    public Task<IReadOnlyList<AccessPolicyDto>> GetPermissions(string id, CancellationToken ct) =>
        mediator.Send(new GetRolePermissionsQuery(id), ct);

    [HttpPost("{id}/permissions")]
    [DynamicAuthorize("Role", "Manage")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AssignPermission(
        string id,
        [FromBody] AssignRolePermissionRequest request,
        CancellationToken ct)
    {
        await mediator.Send(
            new AssignRolePermissionCommand(
                id,
                request.Name,
                request.Resource,
                request.Action,
                request.Effect,
                request.Priority,
                request.ConditionJson,
                request.Description,
                request.IsEnabled),
            ct);
        return NoContent();
    }

    [HttpDelete("{id}/permissions/{policyId:guid}")]
    [DynamicAuthorize("Role", "Manage")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemovePermission(string id, Guid policyId, CancellationToken ct)
    {
        await mediator.Send(new RemoveRolePermissionCommand(id, policyId), ct);
        return NoContent();
    }
}
