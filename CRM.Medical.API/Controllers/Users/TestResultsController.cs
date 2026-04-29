using CRM.Medical.API.Contracts.MedicalWorkflow;
using CRM.Medical.Application.Features.TestResults.DTOs;
using CRM.Medical.Application.Features.TestResults.Services;
using CRM.Medical.Application.Features.Users.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers;

[Authorize]
[ApiController]
[Route("api/test-results")]
public sealed class TestResultsController(ITestResultService testResults) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = UserPermissions.TestResultRead)]
    [ProducesResponseType(typeof(IReadOnlyList<TestResultDto>), StatusCodes.Status200OK)]
    public Task<IReadOnlyList<TestResultDto>> List(
        [FromQuery] int? testRequestId,
        CancellationToken cancellationToken) =>
        testResults.ListAsync(testRequestId, cancellationToken);

    [HttpGet("{id:int}")]
    [Authorize(Policy = UserPermissions.TestResultRead)]
    [ProducesResponseType(typeof(TestResultDto), StatusCodes.Status200OK)]
    public Task<TestResultDto> Get(int id, CancellationToken cancellationToken) =>
        testResults.GetByIdAsync(id, cancellationToken);

    [HttpGet("by-test-request/{testRequestId:int}")]
    [Authorize(Policy = UserPermissions.TestResultRead)]
    [ProducesResponseType(typeof(TestResultDto), StatusCodes.Status200OK)]
    public Task<TestResultDto> GetByTestRequest(int testRequestId, CancellationToken cancellationToken) =>
        testResults.GetByTestRequestIdAsync(testRequestId, cancellationToken);

    [HttpPost("for-test-request/{testRequestId:int}")]
    [Authorize(Policy = UserPermissions.TestResultCreate)]
    [ProducesResponseType(typeof(TestResultDto), StatusCodes.Status200OK)]
    public Task<TestResultDto> Create(
        int testRequestId,
        [FromBody] SaveTestResultRequest request,
        CancellationToken cancellationToken) =>
        testResults.CreateAsync(
            testRequestId,
            request.ResultDate,
            request.ResultData.ToJsonDocument(),
            request.PdfUrl,
            request.Status,
            cancellationToken);

    [HttpPut("{id:int}")]
    [Authorize(Policy = UserPermissions.TestResultUpdate)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] SaveTestResultRequest request,
        CancellationToken cancellationToken)
    {
        await testResults.UpdateAsync(
            id,
            request.ResultDate,
            request.ResultData.ToJsonDocument(),
            request.PdfUrl,
            request.Status,
            cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = UserPermissions.TestResultDelete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await testResults.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
