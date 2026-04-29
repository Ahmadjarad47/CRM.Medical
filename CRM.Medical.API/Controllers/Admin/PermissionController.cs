using CRM.Medical.API.Contracts.Permissions;
using CRM.Medical.Application.Features.Permissions.Services;
using CRM.Medical.Application.Features.Users.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Admin;

[ApiController]
[Authorize(Roles = UserRoles.Admin)]
[Route("api/permissions")]
public sealed class PermissionController(IPermissionService permissionService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct) =>
        Ok(await permissionService.GetPermissionsAsync(ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePermissionRequest request, CancellationToken ct)
    {
        var created = await permissionService.CreateAsync(request.Name, request.Description, ct);
        return Created($"/api/permissions/{created.Id}", created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePermissionRequest request, CancellationToken ct)
    {
        await permissionService.UpdateAsync(id, request.Name, request.Description, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await permissionService.DeleteAsync(id, ct);
        return NoContent();
    }
}
