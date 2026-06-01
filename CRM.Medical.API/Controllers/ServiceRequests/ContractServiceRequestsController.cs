using CRM.Medical.API.Contracts.ServiceRequests;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.ServiceRequests.CQRS;
using CRM.Medical.Application.Features.ServiceRequests.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.ServiceRequests;

[ApiController]
[Route("api/contract-service-requests")]
public sealed class ContractServiceRequestsController(ISender mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ServiceRequestSubmissionResponseDto), StatusCodes.Status200OK)]
    public Task<ServiceRequestSubmissionResponseDto> Submit([FromBody] CreateContractServiceRequest request, CancellationToken ct) =>
        mediator.Send(new SubmitContractServiceRequestCommand(
            request.ContractType,
            request.ResponsibleName,
            request.OrganizationName,
            request.ExpectedSubscribersCount,
            request.ContactNumber,
            request.Email,
            request.Address,
            request.ContractDuration,
            request.AdditionalInfo), ct);

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ContractServiceRequestDto>), StatusCodes.Status200OK)]
    public Task<PagedResult<ContractServiceRequestDto>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        CancellationToken ct = default) =>
        mediator.Send(new ListContractServiceRequestsQuery(page, pageSize, status), ct);

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ContractServiceRequestDto), StatusCodes.Status200OK)]
    public Task<ContractServiceRequestDto> GetById(int id, CancellationToken ct) =>
        mediator.Send(new GetContractServiceRequestQuery(id), ct);

    [HttpPut("{id:int}/status")]
    [ProducesResponseType(typeof(ContractServiceRequestDto), StatusCodes.Status200OK)]
    public Task<ContractServiceRequestDto> UpdateStatus(int id, [FromBody] UpdateRequestStatusRequest request, CancellationToken ct) =>
        mediator.Send(new UpdateContractServiceRequestStatusCommand(id, request.Status, request.Notes), ct);

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await mediator.Send(new DeleteContractServiceRequestCommand(id), ct);
        return NoContent();
    }
}
