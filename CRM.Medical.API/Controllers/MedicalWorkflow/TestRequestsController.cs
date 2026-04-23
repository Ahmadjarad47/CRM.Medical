using CRM.Medical.API.Contracts.MedicalWorkflow.TestRequests;
using CRM.Medical.API.Extensions;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.TestRequests.Commands.CreateTestRequest;
using CRM.Medical.Application.Features.TestRequests.Commands.DeleteTestRequest;
using CRM.Medical.Application.Features.TestRequests.Commands.UpdateTestRequest;
using CRM.Medical.Application.Features.TestRequests.DTOs;
using CRM.Medical.Application.Features.TestRequests.Queries.GetTestRequestById;
using CRM.Medical.Application.Features.TestRequests.Queries.ListTestRequests;
using CRM.Medical.Application.Features.Users.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.MedicalWorkflow;

[ApiController]
[Authorize]
[Route("api/medical-workflow/test-requests")]
public sealed class TestRequestsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = UserPermissions.MedicalWorkflowView)]
    [ProducesResponseType(typeof(PagedResult<TestRequestDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] ListTestRequestsRequest request, CancellationToken ct) =>
        Ok(await mediator.Send(
            new ListTestRequestsQuery(
                request.Page,
                request.PageSize,
                request.MedicalTestId,
                request.Status),
            ct));

    [HttpGet("{id:int}")]
    [Authorize(Policy = UserPermissions.MedicalWorkflowView)]
    [ProducesResponseType(typeof(TestRequestDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetTestRequestByIdQuery(id), ct));

    [HttpPost]
    [Authorize(Policy = UserPermissions.MedicalWorkflowManage)]
    [ProducesResponseType(typeof(TestRequestDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateTestRequestRequest request, CancellationToken ct)
    {
        var dto = await mediator.Send(
            new CreateTestRequestCommand(
                request.MedicalTestId,
                request.RequestDate,
                request.Status,
                request.TotalAmount,
                request.Notes,
                request.Metadata,
                User.GetRequiredUserId()),
            ct);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = UserPermissions.MedicalWorkflowManage)]
    [ProducesResponseType(typeof(TestRequestDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTestRequestRequest request, CancellationToken ct) =>
        Ok(await mediator.Send(
            new UpdateTestRequestCommand(
                id,
                request.RequestDate,
                request.Status,
                request.TotalAmount,
                request.Notes,
                request.Metadata),
            ct));

    [HttpDelete("{id:int}")]
    [Authorize(Policy = UserPermissions.MedicalWorkflowManage)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await mediator.Send(new DeleteTestRequestCommand(id), ct);
        return NoContent();
    }
}
