using CRM.Medical.API.Contracts.Common;
using CRM.Medical.API.Contracts.MedicalWorkflow;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.TestRequests.CQRS;
using CRM.Medical.Application.Features.TestRequests.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Users;

[ApiController]
[Route("api/test-requests")]
public sealed class TestRequestsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<TestRequestDto>), StatusCodes.Status200OK)]
    public Task<PagedResult<TestRequestDto>> List(
        [FromQuery] PagedSearchRequest request,
        CancellationToken cancellationToken) =>
        mediator.Send(
            new ListTestRequestsQuery(request.Page, request.PageSize, request.Search),
            cancellationToken);

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TestRequestDto), StatusCodes.Status200OK)]
    public Task<TestRequestDto> Get(int id, CancellationToken cancellationToken) =>
        mediator.Send(new GetTestRequestByIdQuery(id), cancellationToken);

    [HttpPost]
    [ProducesResponseType(typeof(IReadOnlyList<TestRequestDto>), StatusCodes.Status200OK)]
    public Task<IReadOnlyList<TestRequestDto>> Create(
        [FromBody] CreateTestRequestRequest request,
        CancellationToken cancellationToken) =>
        mediator.Send(
            new CreateTestRequestCommand(
                request.MedicalTestIds,
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteTestRequestCommand(id), cancellationToken);
        return NoContent();
    }
}
