using CRM.Medical.API.Contracts.Permissions;
using CRM.Medical.API.Contracts.Common;
using CRM.Medical.API.Authorization;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Permissions.CQRS;
using CRM.Medical.Application.Features.Permissions.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Admin;

[ApiController]
[Authorize]
[Route("api/permissions")]
public sealed class PermissionController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [DynamicAuthorize("Permission", "View")]
    [ProducesResponseType(typeof(PagedResult<AccessPolicyDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] PagedSearchRequest request, CancellationToken ct) =>
        Ok(await mediator.Send(new ListAccessPoliciesQuery(request.Page, request.PageSize, request.Search), ct));

    [HttpPost]
    [DynamicAuthorize("Permission", "Create")]
    public async Task<IActionResult> Create([FromBody] CreatePermissionRequest request, CancellationToken ct)
    {
        var created = await mediator.Send(
            new CreateAccessPolicyCommand(
                request.Name,
                request.Resource,
                request.Action,
                request.SubjectType,
                request.SubjectId,
                request.Effect,
                request.Priority,
                request.ConditionJson,
                request.Description,
                request.IsEnabled),
            ct);
        return Created($"/api/permissions/{created.Id}", created);
    }

    [HttpPut("{id:guid}")]
    [DynamicAuthorize("Permission", "Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePermissionRequest request, CancellationToken ct)
    {
        await mediator.Send(
            new UpdateAccessPolicyCommand(
                id,
                request.Name,
                request.Resource,
                request.Action,
                request.SubjectType,
                request.SubjectId,
                request.Effect,
                request.Priority,
                request.ConditionJson,
                request.Description,
                request.IsEnabled),
            ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [DynamicAuthorize("Permission", "Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteAccessPolicyCommand(id), ct);
        return NoContent();
    }
}
