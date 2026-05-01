using CRM.Medical.API.Contracts.MedicalWorkflow;
using CRM.Medical.Application.Common.Storage;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.TestResults.DTOs;
using CRM.Medical.Application.Features.TestResults.Services;
using CRM.Medical.Application.Features.Users.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CRM.Medical.API.Controllers;

[Authorize]
[ApiController]
[Route("api/test-results")]
public sealed class TestResultsController(ITestResultService testResults) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = UserPermissions.TestResultRead)]
    [ProducesResponseType(typeof(IReadOnlyList<TestResultDto>), StatusCodes.Status200OK)]
    public Task<IReadOnlyList<TestResultDto>> List(
        [FromQuery] int? testRequestId,
        CancellationToken cancellationToken) =>
        testResults.ListAsync(testRequestId, cancellationToken);

    [HttpGet("{id:int}")]
    [Authorize(Policy = UserPermissions.TestResultRead)]
    [ProducesResponseType(typeof(TestResultDto), StatusCodes.Status200OK)]
    public Task<TestResultDto> Get(int id, CancellationToken cancellationToken) =>
        testResults.GetByIdAsync(id, cancellationToken);

    [HttpGet("by-test-request/{testRequestId:int}")]
    [Authorize(Policy = UserPermissions.TestResultRead)]
    [ProducesResponseType(typeof(TestResultDto), StatusCodes.Status200OK)]
    public Task<TestResultDto> GetByTestRequest(int testRequestId, CancellationToken cancellationToken) =>
        testResults.GetByTestRequestIdAsync(testRequestId, cancellationToken);

    [HttpPost("for-test-request/{testRequestId:int}")]
    [Authorize(Policy = UserPermissions.TestResultCreate)]
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

        return await testResults.CreateAsync(
            testRequestId,
            form.ResultDate,
            resultDataDoc,
            pdfUrl,
            form.Status,
            cancellationToken);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = UserPermissions.TestResultUpdate)]
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

        await testResults.UpdateAsync(
            id,
            form.ResultDate,
            resultDataDoc,
            pdfUrl,
            form.Status,
            cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = UserPermissions.TestResultDelete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await testResults.DeleteAsync(id, cancellationToken);
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
