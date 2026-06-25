using CRM.Medical.API.Contracts.Common;
using CRM.Medical.API.Contracts.MedicalWorkflow;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.CategoryMedical.CQRS;
using CRM.Medical.Application.Features.CategoryMedical.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers;

[ApiController]
[Route("api/category-medical")]
public sealed class CategoryMedicalController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<CategoryMedicalDto>), StatusCodes.Status200OK)]
    public Task<PagedResult<CategoryMedicalDto>> List(
        [FromQuery] PagedSearchRequest request,
        [FromQuery] bool? activeOnly,
        CancellationToken cancellationToken) =>
        mediator.Send(
            new ListCategoryMedicalQuery(request.Page, request.PageSize, request.Search, activeOnly),
            cancellationToken);

    [HttpGet("all")]
    [ProducesResponseType(typeof(IReadOnlyList<CategoryMedicalDto>), StatusCodes.Status200OK)]
    public Task<IReadOnlyList<CategoryMedicalDto>> ListAll(
        [FromQuery] bool activeOnly = true,
        CancellationToken cancellationToken = default) =>
        mediator.Send(new ListAllCategoryMedicalQuery(activeOnly), cancellationToken);

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CategoryMedicalDto), StatusCodes.Status200OK)]
    public Task<CategoryMedicalDto> Get(int id, CancellationToken cancellationToken) =>
        mediator.Send(new GetCategoryMedicalByIdQuery(id), cancellationToken);

    [HttpPost]
    [ProducesResponseType(typeof(CategoryMedicalDto), StatusCodes.Status200OK)]
    public Task<CategoryMedicalDto> Create(
        [FromBody] SaveCategoryMedicalRequest request,
        CancellationToken cancellationToken) =>
        mediator.Send(
            new CreateCategoryMedicalCommand(
                request.NameAr,
                request.NameEn,
                request.Description,
                request.DisplayOrder,
                request.IsActive),
            cancellationToken);

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] SaveCategoryMedicalRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(
            new UpdateCategoryMedicalCommand(
                id,
                request.NameAr,
                request.NameEn,
                request.Description,
                request.DisplayOrder,
                request.IsActive),
            cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteCategoryMedicalCommand(id), cancellationToken);
        return NoContent();
    }
}
