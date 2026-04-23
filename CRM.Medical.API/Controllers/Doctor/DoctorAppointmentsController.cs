using CRM.Medical.API.Contracts.Appointments;
using CRM.Medical.API.Extensions;
using CRM.Medical.API.Mapping;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Appointments;
using CRM.Medical.Application.Features.Appointments.Commands.CancelAppointment;
using CRM.Medical.Application.Features.Appointments.Commands.ConfirmAppointment;
using CRM.Medical.Application.Features.Appointments.DTOs;
using CRM.Medical.Application.Features.Appointments.Queries.GetDoctorAppointmentById;
using CRM.Medical.Application.Features.Appointments.Queries.ListDoctorAppointments;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Doctor;

[Route("api/doctor/appointments")]
public sealed class DoctorAppointmentsController(ISender mediator) : DoctorBaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AppointmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default) =>
        Ok(await mediator.Send(new ListDoctorAppointmentsQuery(User.GetRequiredUserId(), page, pageSize), ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetDoctorAppointmentByIdQuery(User.GetRequiredUserId(), id), ct));

    [HttpPost]
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] DoctorCreateAppointmentRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(
            AppointmentCommandsFromRequests.ToDoctorCreate(User.GetRequiredUserId(), request),
            ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("{id:int}/confirm")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Confirm(int id, CancellationToken ct)
    {
        await mediator.Send(
            new ConfirmAppointmentCommand(id, User.GetRequiredUserId(), AppointmentConfirmationActor.Doctor),
            ct);
        return NoContent();
    }

    [HttpPost("{id:int}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Cancel(int id, CancellationToken ct)
    {
        await mediator.Send(
            new CancelAppointmentCommand(id, User.GetRequiredUserId(), AppointmentCancellationActor.Doctor),
            ct);
        return NoContent();
    }
}
