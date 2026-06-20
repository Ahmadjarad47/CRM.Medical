using CRM.Medical.API.Contracts.MedicalWorkflow;
using CRM.Medical.Application.Features.Appointments.CQRS;
using CRM.Medical.Application.Features.Appointments.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Users;

[ApiController]
[Route("api/appointments")]
public sealed class AppointmentsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AppointmentDto>), StatusCodes.Status200OK)]
    public Task<IReadOnlyList<AppointmentDto>> List(
        [FromQuery] ListAppointmentsRequest request,
        CancellationToken cancellationToken) =>
        mediator.Send(
            new ListAppointmentsQuery(
                request.FromUtc,
                request.ToUtc,
                request.UserId,
                request.Status),
            cancellationToken);

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
    public Task<AppointmentDto> Get(int id, CancellationToken cancellationToken) =>
        mediator.Send(new GetAppointmentByIdQuery(id), cancellationToken);

    [HttpGet("day-availability")]
    [ProducesResponseType(typeof(AppointmentDayAvailabilityDto), StatusCodes.Status200OK)]
    public Task<AppointmentDayAvailabilityDto> GetDayAvailability(
        [FromQuery] GetDayAvailabilityRequest request,
        CancellationToken cancellationToken) =>
        mediator.Send(
            new GetDayAppointmentAvailabilityQuery(
                request.Date,
                request.UserId),
            cancellationToken);

    [HttpPost]
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentRequest request, CancellationToken cancellationToken)
    {
        var dto = await mediator.Send(
            new CreateAppointmentCommand(
                request.AvailabilityId,
                request.TestRequestId,
                request.UserId,
                request.PatientLocationType,
                request.PatientLatitude,
                request.PatientLongitude,
                request.Notes),
            cancellationToken);

        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(
            new UpdateAppointmentCommand(
                id,
                request.AvailabilityId,
                request.TestRequestId,
                request.UserId,
                request.PatientLocationType,
                request.PatientLatitude,
                request.PatientLongitude,
                request.Notes),
            cancellationToken);

        return NoContent();
    }

    [HttpPost("{id:int}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
    {
        await mediator.Send(new CancelAppointmentCommand(id), cancellationToken);
        return NoContent();
    }
}
