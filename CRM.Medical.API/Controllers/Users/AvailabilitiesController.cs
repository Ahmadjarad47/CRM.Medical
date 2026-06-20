using CRM.Medical.API.Contracts.MedicalWorkflow;
using CRM.Medical.Application.Features.Availabilities.CQRS;
using CRM.Medical.Application.Features.Availabilities.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Users;

[ApiController]
[Route("api/availabilities")]
public sealed class AvailabilitiesController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AvailabilityDto>), StatusCodes.Status200OK)]
    public Task<IReadOnlyList<AvailabilityDto>> List([FromQuery] string? userId, CancellationToken cancellationToken) =>
        mediator.Send(new ListAvailabilitiesQuery(userId), cancellationToken);

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AvailabilityDto), StatusCodes.Status200OK)]
    public Task<AvailabilityDto> Get(int id, CancellationToken cancellationToken) =>
        mediator.Send(new GetAvailabilityByIdQuery(id), cancellationToken);

    [HttpPost]
    [ProducesResponseType(typeof(AvailabilityDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] SaveAvailabilityRequest request, CancellationToken cancellationToken)
    {
        var dto = await mediator.Send(
            new CreateAvailabilityCommand(
                request.DayOfWeek,
                request.StartTime,
                request.EndTime,
                request.SlotDuration,
                request.IsActive),
            cancellationToken);

        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] SaveAvailabilityRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(
            new UpdateAvailabilityCommand(
                id,
                request.DayOfWeek,
                request.StartTime,
                request.EndTime,
                request.SlotDuration,
                request.IsActive),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteAvailabilityCommand(id), cancellationToken);
        return NoContent();
    }
}
