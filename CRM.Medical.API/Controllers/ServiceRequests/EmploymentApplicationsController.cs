using CRM.Medical.API.Contracts.ServiceRequests;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.ServiceRequests.CQRS;
using CRM.Medical.Application.Features.ServiceRequests.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.ServiceRequests;

[ApiController]
[Route("api/employment-applications")]
public sealed class EmploymentApplicationsController(ISender mediator) : ControllerBase
{
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ServiceRequestSubmissionResponseDto), StatusCodes.Status200OK)]
    public Task<ServiceRequestSubmissionResponseDto> Submit([FromForm] CreateEmploymentApplicationRequest request, CancellationToken ct) =>
        mediator.Send(new SubmitEmploymentApplicationCommand(
            request.FullName,
            request.ResidencePlace,
            request.MobileNumber,
            request.Email,
            request.AcademicDegree,
            request.PreviousExperience,
            request.YearsOfExperience,
            request.Skills,
            request.AdditionalCertificates,
            request.VacantJobId,
            request.CvFile), ct);

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<EmploymentApplicationRequestDto>), StatusCodes.Status200OK)]
    public Task<PagedResult<EmploymentApplicationRequestDto>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        CancellationToken ct = default) =>
        mediator.Send(new ListEmploymentApplicationsQuery(page, pageSize, status), ct);

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EmploymentApplicationRequestDto), StatusCodes.Status200OK)]
    public Task<EmploymentApplicationRequestDto> GetById(int id, CancellationToken ct) =>
        mediator.Send(new GetEmploymentApplicationQuery(id), ct);

    [HttpPut("{id:int}/status")]
    [ProducesResponseType(typeof(EmploymentApplicationRequestDto), StatusCodes.Status200OK)]
    public Task<EmploymentApplicationRequestDto> UpdateStatus(int id, [FromBody] UpdateRequestStatusRequest request, CancellationToken ct) =>
        mediator.Send(new UpdateEmploymentApplicationStatusCommand(id, request.Status, request.Notes), ct);

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await mediator.Send(new DeleteEmploymentApplicationCommand(id), ct);
        return NoContent();
    }
}
