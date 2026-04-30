using CRM.Medical.API.Contracts.MedicalWorkflow;
using CRM.Medical.Application.Features.TestRequests.DTOs;
using CRM.Medical.Application.Features.TestRequests.Services;
using CRM.Medical.Application.Features.Users.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Users;

[Authorize]
[ApiController]
[Route("api/test-requests")]
public sealed class TestRequestsController(ITestRequestService testRequests) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = UserPermissions.TestRequestRead)]
    [ProducesResponseType(typeof(IReadOnlyList<TestRequestDto>), StatusCodes.Status200OK)]
    public Task<IReadOnlyList<TestRequestDto>> List(CancellationToken cancellationToken) =>
        testRequests.ListAsync(cancellationToken);

    [HttpGet("{id:int}")]
    [Authorize(Policy = UserPermissions.TestRequestRead)]
    [ProducesResponseType(typeof(TestRequestDto), StatusCodes.Status200OK)]
    public Task<TestRequestDto> Get(int id, CancellationToken cancellationToken) =>
        testRequests.GetByIdAsync(id, cancellationToken);

    [HttpPost]
    [Authorize(Policy = UserPermissions.TestRequestCreate)]
    [ProducesResponseType(typeof(TestRequestDto), StatusCodes.Status200OK)]
    public Task<TestRequestDto> Create(
        [FromBody] SaveTestRequestRequest request,
        CancellationToken cancellationToken) =>
        testRequests.CreateAsync(
            request.MedicalTestId,
            request.RequestDate,
            request.Status,
            request.TotalAmount,
            request.Notes,
            request.Metadata.ToJsonDocument(),
            request.DoctorId,
            request.LabClientId,
            request.DirectPatientId,
            request.ExternalPatientId,
            cancellationToken);

    [HttpPut("{id:int}")]
    [Authorize(Policy = UserPermissions.TestRequestUpdate)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] SaveTestRequestRequest request,
        CancellationToken cancellationToken)
    {
        await testRequests.UpdateAsync(
            id,
            request.RequestDate,
            request.Status,
            request.TotalAmount,
            request.Notes,
            request.Metadata.ToJsonDocument(),
            request.DoctorId,
            request.LabClientId,
            request.DirectPatientId,
            request.ExternalPatientId,
            cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = UserPermissions.TestRequestDelete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await testRequests.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
