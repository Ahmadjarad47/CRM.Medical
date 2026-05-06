using CRM.Medical.API.Contracts.MedicalWorkflow;
using CRM.Medical.Application.Common.Storage;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.TestResults.CQRS;
using CRM.Medical.Application.Features.TestResults.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CRM.Medical.API.Controllers;

[ApiController]
[Route("api/test-results")]
public sealed class TestResultsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<TestResultDto>), StatusCodes.Status200OK)]
    public Task<PagedResult<TestResultDto>> List(
        [FromQuery] ListTestResultsRequest request,
        CancellationToken cancellationToken) =>
        mediator.Send(
            new ListTestResultsQuery(
                request.Page,
                request.PageSize,
                request.Search,
                request.TestRequestId),
            cancellationToken);

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TestResultDto), StatusCodes.Status200OK)]
    public Task<TestResultDto> Get(int id, CancellationToken cancellationToken) =>
        mediator.Send(new GetTestResultByIdQuery(id), cancellationToken);

    [HttpGet("by-test-request/{testRequestId:int}")]
    [ProducesResponseType(typeof(TestResultDto), StatusCodes.Status200OK)]
    public Task<TestResultDto> GetByTestRequest(int testRequestId, CancellationToken cancellationToken) =>
        mediator.Send(new GetTestResultByTestRequestIdQuery(testRequestId), cancellationToken);

    [HttpPost("for-test-request/{testRequestId:int}")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(TestResultDto), StatusCodes.Status200OK)]
    public async Task<TestResultDto> Create(
        int testRequestId,
        [FromForm] SaveTestResultForm form,
        [FromServices] IFileStorageService fileStorage,
        CancellationToken cancellationToken)
    {
        var pdfUrl = await ResolvePdfUrlAsync(form.PdfUrl, form.PdfFile, fileStorage, cancellationToken);

        JsonDocument? resultDataDoc;
        try
        {
            resultDataDoc = JsonBodyExtensions.ParseOptionalJsonDocument(form.ResultData);
        }
        catch (JsonException)
        {
            throw new ApplicationBadRequestException("ResultData must be valid JSON when provided.");
        }

        return await mediator.Send(
            new CreateTestResultCommand(
                testRequestId,
                form.ResultDate,
                resultDataDoc,
                pdfUrl,
                form.Status),
            cancellationToken);
    }

    [HttpPut("{id:int}")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(
        int id,
        [FromForm] SaveTestResultForm form,
        [FromServices] IFileStorageService fileStorage,
        CancellationToken cancellationToken)
    {
        var pdfUrl = await ResolvePdfUrlAsync(form.PdfUrl, form.PdfFile, fileStorage, cancellationToken);

        JsonDocument? resultDataDoc;
        try
        {
            resultDataDoc = JsonBodyExtensions.ParseOptionalJsonDocument(form.ResultData);
        }
        catch (JsonException)
        {
            throw new ApplicationBadRequestException("ResultData must be valid JSON when provided.");
        }

        await mediator.Send(
            new UpdateTestResultCommand(
                id,
                form.ResultDate,
                resultDataDoc,
                pdfUrl,
                form.Status),
            cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteTestResultCommand(id), cancellationToken);
        return NoContent();
    }

    private static async Task<string?> ResolvePdfUrlAsync(
        string? pdfUrl,
        IFormFile? pdfFile,
        IFileStorageService fileStorage,
        CancellationToken cancellationToken)
    {
        var hasFile = pdfFile is { Length: > 0 };
        var trimmedUrl = string.IsNullOrWhiteSpace(pdfUrl) ? null : pdfUrl.Trim();

        if (hasFile && trimmedUrl is not null)
            throw new ApplicationBadRequestException("Provide either PdfFile or PdfUrl, not both.");

        if (hasFile)
            return await fileStorage.UploadPdfAsync(pdfFile!, cancellationToken);

        return trimmedUrl;
    }
}
