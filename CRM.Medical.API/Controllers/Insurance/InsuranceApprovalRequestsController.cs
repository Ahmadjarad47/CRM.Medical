using CRM.Medical.API.Contracts.Insurance;
using CRM.Medical.API.Extensions;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Insurance.CQRS;
using CRM.Medical.Application.Features.Insurance.DTOs;
using CRM.Medical.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Insurance;

[ApiController]
[Route("api/insurance-approval-requests")]
public sealed class InsuranceApprovalRequestsController(ISender mediator) : ControllerBase
{
    [Authorize]
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(InsuranceApprovalSubmissionResponseDto), StatusCodes.Status200OK)]
    public Task<InsuranceApprovalSubmissionResponseDto> Submit(
        [FromForm] CreateInsuranceApprovalRequest request,
        CancellationToken ct) =>
        mediator.Send(new SubmitInsuranceApprovalRequestCommand(
            User.GetRequiredUserId(),
            request.InsuredName,
            request.InsuranceNumber,
            request.MobileNumber,
            request.InsuranceCardImage!,
            request.PrescriptionImage!), ct);

    [Authorize]
    [HttpGet("my")]
    [ProducesResponseType(typeof(PagedResult<InsuranceApprovalRequestDto>), StatusCodes.Status200OK)]
    public Task<PagedResult<InsuranceApprovalRequestDto>> MyRequests(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default) =>
        mediator.Send(new ListMyInsuranceApprovalRequestsQuery(User.GetRequiredUserId(), page, pageSize), ct);

    [Authorize]
    [HttpGet("my/{id:int}")]
    [ProducesResponseType(typeof(InsuranceApprovalRequestDetailsDto), StatusCodes.Status200OK)]
    public Task<InsuranceApprovalRequestDetailsDto> MyRequest(int id, CancellationToken ct) =>
        mediator.Send(new GetMyInsuranceApprovalRequestQuery(User.GetRequiredUserId(), id), ct);

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<InsuranceApprovalRequestDto>), StatusCodes.Status200OK)]
    public Task<PagedResult<InsuranceApprovalRequestDto>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] InsuranceApprovalRequestStatus? status = null,
        [FromQuery] string? search = null,
        CancellationToken ct = default) =>
        mediator.Send(new ListInsuranceApprovalRequestsQuery(page, pageSize, status, search), ct);

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(InsuranceApprovalRequestDetailsDto), StatusCodes.Status200OK)]
    public Task<InsuranceApprovalRequestDetailsDto> GetById(int id, CancellationToken ct) =>
        mediator.Send(new GetInsuranceApprovalRequestQuery(id), ct);

    [HttpPut("{id:int}/status")]
    [ProducesResponseType(typeof(InsuranceApprovalRequestDetailsDto), StatusCodes.Status200OK)]
    public Task<InsuranceApprovalRequestDetailsDto> UpdateStatus(
        int id,
        [FromBody] UpdateInsuranceApprovalRequestStatusRequest request,
        CancellationToken ct) =>
        mediator.Send(new UpdateInsuranceApprovalRequestStatusCommand(id, request.Status, request.Notes, request.RejectionReason), ct);

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await mediator.Send(new DeleteInsuranceApprovalRequestCommand(id), ct);
        return NoContent();
    }
}
