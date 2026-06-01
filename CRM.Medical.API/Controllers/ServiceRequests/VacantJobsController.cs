using CRM.Medical.API.Contracts.ServiceRequests;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.ServiceRequests.CQRS;
using CRM.Medical.Application.Features.ServiceRequests.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.ServiceRequests;

[ApiController]
[Route("api/vacant-jobs")]
public sealed class VacantJobsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<VacantJobDto>), StatusCodes.Status200OK)]
    public Task<PagedResult<VacantJobDto>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool includeInactive = false,
        CancellationToken ct = default) =>
        mediator.Send(new ListVacantJobsQuery(page, pageSize, includeInactive), ct);

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(VacantJobDto), StatusCodes.Status200OK)]
    public Task<VacantJobDto> GetById(
        int id,
        [FromQuery] bool includeInactive = false,
        CancellationToken ct = default) =>
        mediator.Send(new GetVacantJobQuery(id, includeInactive), ct);

    [HttpPost]
    [ProducesResponseType(typeof(VacantJobDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] SaveVacantJobRequest request, CancellationToken ct)
    {
        var dto = await mediator.Send(ToCommand(null, request), ct);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id, includeInactive = true }, dto);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(VacantJobDto), StatusCodes.Status200OK)]
    public Task<VacantJobDto> Update(int id, [FromBody] SaveVacantJobRequest request, CancellationToken ct) =>
        mediator.Send(ToCommand(id, request), ct);

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await mediator.Send(new DeleteVacantJobCommand(id), ct);
        return NoContent();
    }

    private static SaveVacantJobCommand ToCommand(int? id, SaveVacantJobRequest request) =>
        new(id, request.TitleAr, request.TitleEn, request.DescriptionAr, request.DescriptionEn, request.IsActive, request.SortOrder);
}
