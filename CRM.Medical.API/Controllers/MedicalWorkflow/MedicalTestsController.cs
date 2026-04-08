using CRM.Medical.API.Controllers.MedicalWorkflow.Models;
using CRM.Medical.API.Extensions;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.MedicalTests.Commands.CreateMedicalTest;
using CRM.Medical.Application.Features.MedicalTests.Commands.DeleteMedicalTest;
using CRM.Medical.Application.Features.MedicalTests.Commands.UpdateMedicalTest;
using CRM.Medical.Application.Features.MedicalTests.DTOs;
using CRM.Medical.Application.Features.MedicalTests.Queries.GetMedicalTestById;
using CRM.Medical.Application.Features.MedicalTests.Queries.ListMedicalTests;
using CRM.Medical.Application.Features.Users.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.MedicalWorkflow;

[ApiController]
[Authorize]
[Route("api/medical-workflow/medical-tests")]
public sealed class MedicalTestsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = UserPermissions.MedicalWorkflowView)]
    [ProducesResponseType(typeof(PagedResult<MedicalTestDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] ListMedicalTestsRequest request, CancellationToken ct) =>
        Ok(await mediator.Send(
            new ListMedicalTestsQuery(request.Page, request.PageSize, request.Category, request.Status),
            ct));

    [HttpGet("{id:int}")]
    [Authorize(Policy = UserPermissions.MedicalWorkflowView)]
    [ProducesResponseType(typeof(MedicalTestDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetMedicalTestByIdQuery(id), ct));

    [HttpPost]
    [Authorize(Policy = UserPermissions.MedicalWorkflowManage)]
    [ProducesResponseType(typeof(MedicalTestDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateMedicalTestRequest request, CancellationToken ct)
    {
        var dto = await mediator.Send(
            new CreateMedicalTestCommand(
                request.NameAr,
                request.NameEn,
                request.Price,
                request.Category,
                request.SampleType,
                request.ParameterSchema,
                request.Status,
                User.GetRequiredUserId()),
            ct);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = UserPermissions.MedicalWorkflowManage)]
    [ProducesResponseType(typeof(MedicalTestDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMedicalTestRequest request, CancellationToken ct) =>
        Ok(await mediator.Send(
            new UpdateMedicalTestCommand(
                id,
                request.NameAr,
                request.NameEn,
                request.Price,
                request.Category,
                request.SampleType,
                request.ParameterSchema,
                request.Status),
            ct));

    [HttpDelete("{id:int}")]
    [Authorize(Policy = UserPermissions.MedicalWorkflowManage)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await mediator.Send(new DeleteMedicalTestCommand(id), ct);
        return NoContent();
    }
}
