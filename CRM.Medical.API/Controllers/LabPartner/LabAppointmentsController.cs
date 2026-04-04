using CRM.Medical.API.Extensions;
using CRM.Medical.API.Mapping;
using CRM.Medical.API.Models.Appointments;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Appointments;
using CRM.Medical.Application.Features.Appointments.Commands.CancelAppointment;
using CRM.Medical.Application.Features.Appointments.Commands.ConfirmAppointment;
using CRM.Medical.Application.Features.Appointments.DTOs;
using CRM.Medical.Application.Features.Appointments.Queries.GetLabAppointmentById;
using CRM.Medical.Application.Features.Appointments.Queries.ListLabAppointments;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.LabPartner;

[Route("api/labpartner/appointments")]
public sealed class LabAppointmentsController(ISender mediator) : LabPartnerBaseController
{
    [HttpPost]
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] LabCreateAppointmentRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(
            AppointmentCommandsFromRequests.ToLabCreate(User.GetRequiredUserId(), request),
            ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AppointmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default) =>
        Ok(await mediator.Send(new ListLabAppointmentsQuery(User.GetRequiredUserId(), page, pageSize), ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetLabAppointmentByIdQuery(User.GetRequiredUserId(), id), ct));

    [HttpPost("{id:int}/confirm")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Confirm(int id, CancellationToken ct)
    {
        await mediator.Send(
            new ConfirmAppointmentCommand(id, User.GetRequiredUserId(), AppointmentConfirmationActor.LabPartner),
            ct);
        return NoContent();
    }

    [HttpPost("{id:int}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Cancel(int id, CancellationToken ct)
    {
        await mediator.Send(
            new CancelAppointmentCommand(id, User.GetRequiredUserId(), AppointmentCancellationActor.LabPartner),
            ct);
        return NoContent();
    }
}
