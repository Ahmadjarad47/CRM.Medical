using CRM.Medical.API.Contracts.Common;
using CRM.Medical.API.Contracts.MedicalWorkflow;
using CRM.Medical.API.Authorization;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.MedicalTests.CQRS;
using CRM.Medical.Application.Features.MedicalTests.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers;

[Authorize]
[ApiController]
[Route("api/medical-tests")]
public sealed class MedicalTestsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [DynamicAuthorize("MedicalTest", "Read")]
    [ProducesResponseType(typeof(PagedResult<MedicalTestDto>), StatusCodes.Status200OK)]
    public Task<PagedResult<MedicalTestDto>> List(
        [FromQuery] PagedSearchRequest request,
        CancellationToken cancellationToken) =>
        mediator.Send(
            new ListMedicalTestsQuery(request.Page, request.PageSize, request.Search),
            cancellationToken);

    [HttpGet("{id:int}")]
    [DynamicAuthorize("MedicalTest", "Read")]
    [ProducesResponseType(typeof(MedicalTestDto), StatusCodes.Status200OK)]
    public Task<MedicalTestDto> Get(int id, CancellationToken cancellationToken) =>
        mediator.Send(new GetMedicalTestByIdQuery(id), cancellationToken);

    [HttpPost]
    [DynamicAuthorize("MedicalTest", "Create")]
    [ProducesResponseType(typeof(MedicalTestDto), StatusCodes.Status200OK)]
    public Task<MedicalTestDto> Create(
        [FromBody] CreateMedicalTestRequest request,
        CancellationToken cancellationToken) =>
        mediator.Send(
            new CreateMedicalTestCommand(
                request.NameAr,
                request.NameEn,
                request.Price,
                request.Category,
                request.SampleType,
                request.ParameterSchema.ToJsonDocument(),
                request.Status),
            cancellationToken);

    [HttpPut("{id:int}")]
    [DynamicAuthorize("MedicalTest", "Update")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateMedicalTestRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(
            new UpdateMedicalTestCommand(
                id,
                request.NameAr,
                request.NameEn,
                request.Price,
                request.Category,
                request.SampleType,
                request.ParameterSchema.ToJsonDocument(),
                request.Status),
            cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [DynamicAuthorize("MedicalTest", "Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteMedicalTestCommand(id), cancellationToken);
        return NoContent();
    }
}
