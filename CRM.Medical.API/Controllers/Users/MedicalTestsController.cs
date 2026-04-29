using CRM.Medical.API.Contracts.MedicalWorkflow;
using CRM.Medical.Application.Features.MedicalTests.DTOs;
using CRM.Medical.Application.Features.MedicalTests.Services;
using CRM.Medical.Application.Features.Users.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers;

[Authorize]
[ApiController]
[Route("api/medical-tests")]
public sealed class MedicalTestsController(IMedicalTestService medicalTests) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = UserPermissions.MedicalTestRead)]
    [ProducesResponseType(typeof(IReadOnlyList<MedicalTestDto>), StatusCodes.Status200OK)]
    public Task<IReadOnlyList<MedicalTestDto>> List(CancellationToken cancellationToken) =>
        medicalTests.ListAsync(cancellationToken);

    [HttpGet("{id:int}")]
    [Authorize(Policy = UserPermissions.MedicalTestRead)]
    [ProducesResponseType(typeof(MedicalTestDto), StatusCodes.Status200OK)]
    public Task<MedicalTestDto> Get(int id, CancellationToken cancellationToken) =>
        medicalTests.GetByIdAsync(id, cancellationToken);

    [HttpPost]
    [Authorize(Policy = UserPermissions.MedicalTestCreate)]
    [ProducesResponseType(typeof(MedicalTestDto), StatusCodes.Status200OK)]
    public Task<MedicalTestDto> Create(
        [FromBody] CreateMedicalTestRequest request,
        CancellationToken cancellationToken) =>
        medicalTests.CreateAsync(
            request.NameAr,
            request.NameEn,
            request.Price,
            request.Category,
            request.SampleType,
            request.ParameterSchema.ToJsonDocument(),
            request.Status,
            cancellationToken);

    [HttpPut("{id:int}")]
    [Authorize(Policy = UserPermissions.MedicalTestUpdate)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateMedicalTestRequest request,
        CancellationToken cancellationToken)
    {
        await medicalTests.UpdateAsync(
            id,
            request.NameAr,
            request.NameEn,
            request.Price,
            request.Category,
            request.SampleType,
            request.ParameterSchema.ToJsonDocument(),
            request.Status,
            cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = UserPermissions.MedicalTestDelete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await medicalTests.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
