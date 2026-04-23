using CRM.Medical.API.Contracts.Admin.Complaints;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Complaints.Commands.UpdateComplaintStatus;
using CRM.Medical.Application.Features.Complaints.DTOs;
using CRM.Medical.Application.Features.Complaints.Queries.ListComplaints;
using CRM.Medical.Application.Features.Users.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Admin;

[Route("api/admin/complaints")]
public sealed class ComplaintsController(ISender mediator) : AdminBaseController
{
    [HttpGet]
    [Authorize(Policy = UserPermissions.ComplaintsView)]
    [ProducesResponseType(typeof(PagedResult<ComplaintDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] ListComplaintsRequest request, CancellationToken ct) =>
        Ok(await mediator.Send(
            new ListComplaintsQuery(request.Page, request.PageSize, request.Status, request.UserId),
            ct));

    [HttpPut("{id:int}/status")]
    [Authorize(Policy = UserPermissions.ComplaintsUpdateStatus)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateComplaintStatusRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateComplaintStatusCommand(id, request.Status), ct);
        return NoContent();
    }
}
