using CRM.Medical.API.Authorization;
using CRM.Medical.Application.Features.Users.Queries.GetUserPermissions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Users;

[ApiController]
[Authorize]
[Route("api/users")]
public sealed class UserPermissionController(ISender mediator) : ControllerBase
{
    [HttpGet("{userId}/permissions")]
    [DynamicAuthorize("User", "View")]
    public async Task<IActionResult> GetUserPermissions(string userId, CancellationToken ct) =>
        Ok(await mediator.Send(new GetUserPermissionsQuery(userId), ct));
}
