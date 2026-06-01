using CRM.Medical.API.Contracts.Accounting;
using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Accounting.CQRS;
using CRM.Medical.Application.Features.Accounting.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Medical.API.Controllers.Accounting;

[ApiController]
[Route("api/accounting")]
public sealed class AccountingController(ISender mediator) : ControllerBase
{
    [HttpGet("settings")]
    [ProducesResponseType(typeof(AccountingPageSettingDto), StatusCodes.Status200OK)]
    public Task<AccountingPageSettingDto> GetSettings(CancellationToken ct) =>
        mediator.Send(new GetAccountingSettingsQuery(), ct);

    [HttpPut("settings")]
    [ProducesResponseType(typeof(AccountingPageSettingDto), StatusCodes.Status200OK)]
    public Task<AccountingPageSettingDto> UpdateSettings(
        [FromBody] UpdateAccountingPageSettingRequest request,
        CancellationToken ct) =>
        mediator.Send(new UpdateAccountingSettingsCommand(
            request.AnnouncementTextAr,
            request.AnnouncementTextEn,
            request.TitleAr,
            request.TitleEn,
            request.DescriptionAr,
            request.DescriptionEn,
            request.IsActive), ct);

    [HttpGet("statements")]
    [ProducesResponseType(typeof(LabAccountStatementDto), StatusCodes.Status200OK)]
    public Task<LabAccountStatementDto> GetStatement(
        [FromQuery] string labClientId,
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken ct) =>
        mediator.Send(new GetLabAccountStatementQuery(labClientId, from, to), ct);

    [HttpGet("statements/pdf")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> DownloadStatementPdf(
        [FromQuery] string labClientId,
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken ct)
    {
        var pdf = await mediator.Send(new GenerateLabAccountStatementPdfQuery(labClientId, from, to), ct);
        return File(pdf, "application/pdf", $"lab-account-statement-{from:yyyyMMdd}-{to:yyyyMMdd}.pdf");
    }

    [HttpPost("statements/pdf")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(LabAccountStatementFileDto), StatusCodes.Status200OK)]
    public Task<LabAccountStatementFileDto> UploadStatementPdf(
        [FromForm] UploadLabAccountStatementPdfRequest request,
        CancellationToken ct) =>
        mediator.Send(new UploadLabAccountStatementPdfCommand(request.LabClientId, request.From, request.To, request.File, request.Notes), ct);

    [HttpGet("payments")]
    [ProducesResponseType(typeof(PagedResult<LabAccountPaymentDto>), StatusCodes.Status200OK)]
    public Task<PagedResult<LabAccountPaymentDto>> ListPayments(
        [FromQuery] string? labClientId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default) =>
        mediator.Send(new ListLabAccountPaymentsQuery(labClientId, from, to, page, pageSize), ct);

    [HttpPost("payments")]
    [ProducesResponseType(typeof(LabAccountPaymentDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreatePayment([FromBody] SaveLabAccountPaymentRequest request, CancellationToken ct)
    {
        var dto = await mediator.Send(ToCommand(null, request), ct);
        return StatusCode(StatusCodes.Status201Created, dto);
    }

    [HttpPut("payments/{id:int}")]
    [ProducesResponseType(typeof(LabAccountPaymentDto), StatusCodes.Status200OK)]
    public Task<LabAccountPaymentDto> UpdatePayment(
        int id,
        [FromBody] SaveLabAccountPaymentRequest request,
        CancellationToken ct) =>
        mediator.Send(ToCommand(id, request), ct);

    [HttpDelete("payments/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeletePayment(int id, CancellationToken ct)
    {
        await mediator.Send(new DeleteLabAccountPaymentCommand(id), ct);
        return NoContent();
    }

    private static SaveLabAccountPaymentCommand ToCommand(int? id, SaveLabAccountPaymentRequest request) =>
        new(id, request.LabClientId, request.Amount, request.PaidAt, request.PaymentMethod, request.ReferenceNumber, request.Notes);
}
