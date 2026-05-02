using CRM.Medical.API.Contracts.MedicalWorkflow;
using CRM.Medical.Application.Features.ExternalPatients.DTOs;
using CRM.Medical.Application.Features.ExternalPatients.Services;
using CRM.Medical.Application.Features.Users.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Users;

[Authorize]
[ApiController]
[Route("api/external-patients")]
public sealed class ExternalPatientsController(IExternalPatientService externalPatients) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = UserPermissions.ExternalPatientsManage)]
    [ProducesResponseType(typeof(IReadOnlyList<ExternalPatientDto>), StatusCodes.Status200OK)]
    public Task<IReadOnlyList<ExternalPatientDto>> List(CancellationToken cancellationToken) =>
        externalPatients.ListAsync(cancellationToken);

    [HttpGet("{id:int}")]
    [Authorize(Policy = UserPermissions.ExternalPatientsManage)]
    [ProducesResponseType(typeof(ExternalPatientDto), StatusCodes.Status200OK)]
    public Task<ExternalPatientDto> Get(int id, CancellationToken cancellationToken) =>
        externalPatients.GetByIdAsync(id, cancellationToken);

    [HttpPost]
    [Authorize(Policy = UserPermissions.ExternalPatientsManage)]
    [ProducesResponseType(typeof(ExternalPatientDto), StatusCodes.Status200OK)]
    public Task<ExternalPatientDto> Create(
        [FromBody] SaveExternalPatientRequest request,
        CancellationToken cancellationToken) =>
        externalPatients.CreateAsync(
            request.FullName,
            request.Age,
            request.Gender,
            request.PhoneNumber,
            request.ExternalId,
            cancellationToken);

    [HttpPost("{id:int}/link-direct-patient")]
    [Authorize(Policy = UserPermissions.ExternalPatientsManage)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> LinkDirectPatient(
        int id,
        [FromBody] LinkExternalPatientRequest request,
        CancellationToken cancellationToken)
    {
        await externalPatients.LinkToDirectPatientAsync(id, request.DirectPatientUserId, cancellationToken);
        return NoContent();
    }
}
