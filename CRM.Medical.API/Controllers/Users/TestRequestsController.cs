using CRM.Medical.API.Contracts.Common;
using CRM.Medical.API.Contracts.MedicalWorkflow;
using CRM.Medical.API.Authorization;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.TestRequests.CQRS;
using CRM.Medical.Application.Features.TestRequests.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Users;

[Authorize]
[ApiController]
[Route("api/test-requests")]
public sealed class TestRequestsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [DynamicAuthorize("TestRequest", "Read")]
    [ProducesResponseType(typeof(PagedResult<TestRequestDto>), StatusCodes.Status200OK)]
    public Task<PagedResult<TestRequestDto>> List(
        [FromQuery] PagedSearchRequest request,
        CancellationToken cancellationToken) =>
        mediator.Send(
            new ListTestRequestsQuery(request.Page, request.PageSize, request.Search),
            cancellationToken);

    [HttpGet("{id:int}")]
    [DynamicAuthorize("TestRequest", "Read")]
    [ProducesResponseType(typeof(TestRequestDto), StatusCodes.Status200OK)]
    public Task<TestRequestDto> Get(int id, CancellationToken cancellationToken) =>
        mediator.Send(new GetTestRequestByIdQuery(id), cancellationToken);

    [HttpPost]
    [DynamicAuthorize("TestRequest", "Create")]
    [ProducesResponseType(typeof(TestRequestDto), StatusCodes.Status200OK)]
    public Task<TestRequestDto> Create(
        [FromBody] SaveTestRequestRequest request,
        CancellationToken cancellationToken) =>
        mediator.Send(
            new CreateTestRequestCommand(
                request.MedicalTestId,
                request.RequestDate,
                request.Status,
                request.TotalAmount,
                request.Notes,
                request.Metadata.ToJsonDocument(),
                request.DoctorId,
                request.LabClientId,
                request.DirectPatientId,
                request.ExternalPatientId),
            cancellationToken);

    [HttpPut("{id:int}")]
    [DynamicAuthorize("TestRequest", "Update")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] SaveTestRequestRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(
            new UpdateTestRequestCommand(
                id,
                request.RequestDate,
                request.Status,
                request.TotalAmount,
                request.Notes,
                request.Metadata.ToJsonDocument(),
                request.DoctorId,
                request.LabClientId,
                request.DirectPatientId,
                request.ExternalPatientId),
            cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [DynamicAuthorize("TestRequest", "Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteTestRequestCommand(id), cancellationToken);
        return NoContent();
    }
}
