using CRM.Medical.API.Contracts.User.Complaints;
using CRM.Medical.API.Extensions;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Complaints.Commands.SubmitComplaint;
using CRM.Medical.Application.Features.Complaints.DTOs;
using CRM.Medical.Application.Features.Complaints.Queries.GetMyComplaintById;
using CRM.Medical.Application.Features.Complaints.Queries.GetMyComplaints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.User;

[Route("api/users/me/complaints")]
public sealed class MeComplaintsController(ISender mediator) : UserBaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ComplaintDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default) =>
        Ok(await mediator.Send(new GetMyComplaintsQuery(User.GetRequiredUserId(), page, pageSize), ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ComplaintDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetMyComplaintByIdQuery(User.GetRequiredUserId(), id), ct));

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ComplaintDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Submit([FromForm] SubmitComplaintRequest request, CancellationToken ct)
    {
        var command = new SubmitComplaintCommand(
            User.GetRequiredUserId(),
            request.Title,
            request.Description,
            request.Attachment);
        var result = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}
