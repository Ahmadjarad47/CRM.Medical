using CRM.Medical.Application.Features.AppointmentTypes.DTOs;
using CRM.Medical.Application.Features.AppointmentTypes.Queries.ListActiveAppointmentTypes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers;

/// <summary>Active appointment types for booking (catalog defined by administrators).</summary>
[Authorize]
[ApiController]
[Route("api/appointment-types")]
public sealed class AppointmentTypesController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AppointmentTypeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListActive(CancellationToken ct) =>
        Ok(await mediator.Send(new ListActiveAppointmentTypesQuery(), ct));
}
