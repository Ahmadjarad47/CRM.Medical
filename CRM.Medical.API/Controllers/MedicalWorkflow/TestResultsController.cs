using CRM.Medical.API.Controllers.MedicalWorkflow.Models;
using CRM.Medical.API.Extensions;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.TestResults.Commands.CreateTestResult;
using CRM.Medical.Application.Features.TestResults.Commands.DeleteTestResult;
using CRM.Medical.Application.Features.TestResults.Commands.UpdateTestResult;
using CRM.Medical.Application.Features.TestResults.DTOs;
using CRM.Medical.Application.Features.TestResults.Queries.GetTestResultById;
using CRM.Medical.Application.Features.TestResults.Queries.ListTestResults;
using CRM.Medical.Application.Features.Users.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.MedicalWorkflow;

[ApiController]
[Authorize]
[Route("api/medical-workflow/test-results")]
public sealed class TestResultsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = UserPermissions.MedicalWorkflowView)]
    [ProducesResponseType(typeof(PagedResult<TestResultDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] ListTestResultsRequest request, CancellationToken ct) =>
        Ok(await mediator.Send(
            new ListTestResultsQuery(
                request.Page,
                request.PageSize,
                request.TestRequestId,
                request.Status),
            ct));

    [HttpGet("{id:int}")]
    [Authorize(Policy = UserPermissions.MedicalWorkflowView)]
    [ProducesResponseType(typeof(TestResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetTestResultByIdQuery(id), ct));

    [HttpPost]
    [Authorize(Policy = UserPermissions.MedicalWorkflowManage)]
    [ProducesResponseType(typeof(TestResultDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateTestResultRequest request, CancellationToken ct)
    {
        var dto = await mediator.Send(
            new CreateTestResultCommand(
                request.TestRequestId,
                request.ResultDate,
                request.ResultData,
                request.PdfUrl,
                request.Status,
                User.GetRequiredUserId()),
            ct);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = UserPermissions.MedicalWorkflowManage)]
    [ProducesResponseType(typeof(TestResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTestResultRequest request, CancellationToken ct) =>
        Ok(await mediator.Send(
            new UpdateTestResultCommand(
                id,
                request.ResultDate,
                request.ResultData,
                request.PdfUrl,
                request.Status),
            ct));

    [HttpDelete("{id:int}")]
    [Authorize(Policy = UserPermissions.MedicalWorkflowManage)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await mediator.Send(new DeleteTestResultCommand(id), ct);
        return NoContent();
    }
}
