using CRM.Medical.API.Contracts.Admin.Appointments;
using CRM.Medical.API.Contracts.Appointments;
using CRM.Medical.API.Extensions;
using CRM.Medical.API.Mapping;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Appointments;
using CRM.Medical.Application.Features.Appointments.Commands.CancelAppointment;
using CRM.Medical.Application.Features.Appointments.Commands.ConfirmAppointment;
using CRM.Medical.Application.Features.Appointments.DTOs;
using CRM.Medical.Application.Features.Appointments.Queries.GetAdminAppointmentById;
using CRM.Medical.Application.Features.Appointments.Queries.ListAdminAppointments;
using CRM.Medical.Application.Features.Users.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Admin;

[Route("api/admin/appointments")]
public sealed class AppointmentsController(ISender mediator) : AdminBaseController
{
    [HttpGet]
    [Authorize(Policy = UserPermissions.AppointmentsView)]
    [ProducesResponseType(typeof(PagedResult<AppointmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] AppointmentAdminListRequest request, CancellationToken ct) =>
        Ok(await mediator.Send(
            new ListAdminAppointmentsQuery(
                request.Page,
                request.PageSize,
                request.PatientId,
                request.DoctorId,
                request.LabPartnerId,
                request.Status),
            ct));

    [HttpGet("{id:int}")]
    [Authorize(Policy = UserPermissions.AppointmentsView)]
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetAdminAppointmentByIdQuery(id), ct));

    [HttpPost]
    [Authorize(Policy = UserPermissions.AppointmentsManage)]
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] AdminCreateAppointmentRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(
            AppointmentCommandsFromRequests.ToAdminCreate(User.GetRequiredUserId(), request),
            ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("{id:int}/confirm")]
    [Authorize(Policy = UserPermissions.AppointmentsManage)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Confirm(int id, CancellationToken ct)
    {
        await mediator.Send(
            new ConfirmAppointmentCommand(id, User.GetRequiredUserId(), AppointmentConfirmationActor.Admin),
            ct);
        return NoContent();
    }

    [HttpPost("{id:int}/cancel")]
    [Authorize(Policy = UserPermissions.AppointmentsManage)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Cancel(int id, CancellationToken ct)
    {
        await mediator.Send(
            new CancelAppointmentCommand(id, User.GetRequiredUserId(), AppointmentCancellationActor.Admin),
            ct);
        return NoContent();
    }
}
