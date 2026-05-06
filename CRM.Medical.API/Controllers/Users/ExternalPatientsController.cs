using CRM.Medical.API.Contracts.Common;
using CRM.Medical.API.Contracts.MedicalWorkflow;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.ExternalPatients.CQRS;
using CRM.Medical.Application.Features.ExternalPatients.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Users;

[ApiController]
[Route("api/external-patients")]
public sealed class ExternalPatientsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ExternalPatientDto>), StatusCodes.Status200OK)]
    public Task<PagedResult<ExternalPatientDto>> List(
        [FromQuery] PagedSearchRequest request,
        CancellationToken cancellationToken) =>
        mediator.Send(
            new ListExternalPatientsQuery(request.Page, request.PageSize, request.Search),
            cancellationToken);

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ExternalPatientDto), StatusCodes.Status200OK)]
    public Task<ExternalPatientDto> Get(int id, CancellationToken cancellationToken) =>
        mediator.Send(new GetExternalPatientByIdQuery(id), cancellationToken);

    [HttpPost]
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
