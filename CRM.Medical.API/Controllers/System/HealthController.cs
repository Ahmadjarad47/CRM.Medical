using CRM.Medical.Application.Features.Health.GetStatus;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        return StatusCode(result.HttpStatusCode, result);
    }
}
