using CRM.Medical.API.Contracts.ServiceRequests;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.ServiceRequests.CQRS;
using CRM.Medical.Application.Features.ServiceRequests.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.ServiceRequests;

[ApiController]
[Route("api/client-join-requests")]
public sealed class ClientJoinRequestsController(ISender mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ServiceRequestSubmissionResponseDto), StatusCodes.Status200OK)]
    public Task<ServiceRequestSubmissionResponseDto> Submit([FromBody] CreateClientJoinRequest request, CancellationToken ct) =>
        mediator.Send(new SubmitClientJoinRequestCommand(
            request.ManagerName,
            request.LabName,
            request.MobileNumber,
            request.Email,
            request.Address,
            request.AdditionalInfo), ct);

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ClientJoinRequestDto>), StatusCodes.Status200OK)]
    public Task<PagedResult<ClientJoinRequestDto>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        CancellationToken ct = default) =>
        mediator.Send(new ListClientJoinRequestsQuery(page, pageSize, status), ct);

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ClientJoinRequestDto), StatusCodes.Status200OK)]
    public Task<ClientJoinRequestDto> GetById(int id, CancellationToken ct) =>
        mediator.Send(new GetClientJoinRequestQuery(id), ct);

    [HttpPut("{id:int}/status")]
    [ProducesResponseType(typeof(ClientJoinRequestDto), StatusCodes.Status200OK)]
    public Task<ClientJoinRequestDto> UpdateStatus(int id, [FromBody] UpdateRequestStatusRequest request, CancellationToken ct) =>
        mediator.Send(new UpdateClientJoinRequestStatusCommand(id, request.Status, request.Notes), ct);

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await mediator.Send(new DeleteClientJoinRequestCommand(id), ct);
        return NoContent();
    }
}
