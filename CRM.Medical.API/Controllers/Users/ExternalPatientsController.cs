using CRM.Medical.API.Contracts.Common;
using CRM.Medical.API.Contracts.MedicalWorkflow;
using CRM.Medical.API.Authorization;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.ExternalPatients.CQRS;
using CRM.Medical.Application.Features.ExternalPatients.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Users;

[Authorize]
[ApiController]
[Route("api/external-patients")]
public sealed class ExternalPatientsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [DynamicAuthorize("ExternalPatient", "Manage")]
    [ProducesResponseType(typeof(PagedResult<ExternalPatientDto>), StatusCodes.Status200OK)]
    public Task<PagedResult<ExternalPatientDto>> List(
        [FromQuery] PagedSearchRequest request,
        CancellationToken cancellationToken) =>
        mediator.Send(
            new ListExternalPatientsQuery(request.Page, request.PageSize, request.Search),
            cancellationToken);

    [HttpGet("{id:int}")]
    [DynamicAuthorize("ExternalPatient", "Manage")]
    [ProducesResponseType(typeof(ExternalPatientDto), StatusCodes.Status200OK)]
    public Task<ExternalPatientDto> Get(int id, CancellationToken cancellationToken) =>
        mediator.Send(new GetExternalPatientByIdQuery(id), cancellationToken);

    [HttpPost]
    [DynamicAuthorize("ExternalPatient", "Manage")]
    [ProducesResponseType(typeof(ExternalPatientDto), StatusCodes.Status200OK)]
    public Task<ExternalPatientDto> Create(
        [FromBody] SaveExternalPatientRequest request,
        CancellationToken cancellationToken) =>
        mediator.Send(
            new CreateExternalPatientCommand(
                request.FullName,
                request.Age,
                request.Gender,
                request.PhoneNumber,
                request.ExternalId),
            cancellationToken);

    [HttpPost("{id:int}/link-direct-patient")]
    [DynamicAuthorize("ExternalPatient", "Manage")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> LinkDirectPatient(
        int id,
        [FromBody] LinkExternalPatientRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(
            new LinkExternalPatientToDirectPatientCommand(id, request.DirectPatientUserId),
            cancellationToken);
        return NoContent();
    }
}
