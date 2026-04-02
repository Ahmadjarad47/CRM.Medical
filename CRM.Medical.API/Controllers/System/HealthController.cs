using CRM.Medical.Application.Features.Health.GetStatus;
using Microsoft.AspNetCore.Authorization;

namespace CRM.Medical.API.Controllers.System;

[AllowAnonymous]
[Route("health")]
public sealed class HealthController(ISender mediator) : SystemBaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(HealthStatusViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HealthStatusViewModel), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetStatus(CancellationToken ct)
    {
        var result = await mediator.Send(new GetHealthStatusQuery(), ct);
        var statusCode = result.Status == "Healthy"
            ? StatusCodes.Status200OK
            : StatusCodes.Status503ServiceUnavailable;
        return StatusCode(statusCode, result);
    }
}
