using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Accounting.DTOs;
using CRM.Medical.Application.Features.Accounting.Services;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CRM.Medical.Application.Features.Accounting.CQRS;

public sealed record GetAccountingSettingsQuery : IRequest<AccountingPageSettingDto>;
public sealed class GetAccountingSettingsQueryHandler(IAccountingService service) : IRequestHandler<GetAccountingSettingsQuery, AccountingPageSettingDto>
{
    public Task<AccountingPageSettingDto> Handle(GetAccountingSettingsQuery request, CancellationToken cancellationToken) =>
        service.GetSettingsAsync(cancellationToken);
}

public sealed record UpdateAccountingSettingsCommand(string AnnouncementTextAr, string AnnouncementTextEn, string TitleAr, string TitleEn, string DescriptionAr, string DescriptionEn, bool IsActive) : IRequest<AccountingPageSettingDto>;
public sealed class UpdateAccountingSettingsCommandHandler(IAccountingService service) : IRequestHandler<UpdateAccountingSettingsCommand, AccountingPageSettingDto>
{
    public Task<AccountingPageSettingDto> Handle(UpdateAccountingSettingsCommand r, CancellationToken ct) =>
        service.UpdateSettingsAsync(r.AnnouncementTextAr, r.AnnouncementTextEn, r.TitleAr, r.TitleEn, r.DescriptionAr, r.DescriptionEn, r.IsActive, ct);
}

public sealed record GetLabAccountStatementQuery(string LabClientId, DateTime From, DateTime To) : IRequest<LabAccountStatementDto>;
public sealed class GetLabAccountStatementQueryHandler(IAccountingService service) : IRequestHandler<GetLabAccountStatementQuery, LabAccountStatementDto>
{
    public Task<LabAccountStatementDto> Handle(GetLabAccountStatementQuery request, CancellationToken cancellationToken) =>
        service.GetStatementAsync(request.LabClientId, request.From, request.To, cancellationToken);
}

public sealed record GenerateLabAccountStatementPdfQuery(string LabClientId, DateTime From, DateTime To) : IRequest<byte[]>;
public sealed class GenerateLabAccountStatementPdfQueryHandler(IAccountingService service) : IRequestHandler<GenerateLabAccountStatementPdfQuery, byte[]>
{
    public Task<byte[]> Handle(GenerateLabAccountStatementPdfQuery request, CancellationToken cancellationToken) =>
        service.GenerateStatementPdfAsync(request.LabClientId, request.From, request.To, cancellationToken);
}

public sealed record UploadLabAccountStatementPdfCommand(string LabClientId, DateTime From, DateTime To, IFormFile File, string? Notes) : IRequest<LabAccountStatementFileDto>;
public sealed class UploadLabAccountStatementPdfCommandHandler(IAccountingService service) : IRequestHandler<UploadLabAccountStatementPdfCommand, LabAccountStatementFileDto>
{
    public Task<LabAccountStatementFileDto> Handle(UploadLabAccountStatementPdfCommand request, CancellationToken cancellationToken) =>
        service.UploadStatementPdfAsync(request.LabClientId, request.From, request.To, request.File, request.Notes, cancellationToken);
}

public sealed record ListLabAccountPaymentsQuery(string? LabClientId, DateTime? From, DateTime? To, int Page, int PageSize) : IRequest<PagedResult<LabAccountPaymentDto>>;
public sealed class ListLabAccountPaymentsQueryHandler(IAccountingService service) : IRequestHandler<ListLabAccountPaymentsQuery, PagedResult<LabAccountPaymentDto>>
{
    public Task<PagedResult<LabAccountPaymentDto>> Handle(ListLabAccountPaymentsQuery request, CancellationToken cancellationToken) =>
        service.ListPaymentsAsync(request.LabClientId, request.From, request.To, request.Page, request.PageSize, cancellationToken);
}

public sealed record SaveLabAccountPaymentCommand(int? Id, string LabClientId, decimal Amount, DateTime PaidAt, string PaymentMethod, string? ReferenceNumber, string? Notes) : IRequest<LabAccountPaymentDto>;
public sealed class SaveLabAccountPaymentCommandHandler(IAccountingService service) : IRequestHandler<SaveLabAccountPaymentCommand, LabAccountPaymentDto>
{
    public Task<LabAccountPaymentDto> Handle(SaveLabAccountPaymentCommand r, CancellationToken ct) =>
        r.Id is null
            ? service.CreatePaymentAsync(r.LabClientId, r.Amount, r.PaidAt, r.PaymentMethod, r.ReferenceNumber, r.Notes, ct)
            : service.UpdatePaymentAsync(r.Id.Value, r.LabClientId, r.Amount, r.PaidAt, r.PaymentMethod, r.ReferenceNumber, r.Notes, ct);
}

public sealed record DeleteLabAccountPaymentCommand(int Id) : IRequest;
public sealed class DeleteLabAccountPaymentCommandHandler(IAccountingService service) : IRequestHandler<DeleteLabAccountPaymentCommand>
{
    public Task Handle(DeleteLabAccountPaymentCommand request, CancellationToken cancellationToken) =>
        service.DeletePaymentAsync(request.Id, cancellationToken);
}
