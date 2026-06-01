using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.Insurance.DTOs;
using CRM.Medical.Application.Features.Insurance.Services;
using CRM.Medical.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CRM.Medical.Application.Features.Insurance.CQRS;

public sealed record SubmitInsuranceApprovalRequestCommand(string PatientId, string InsuredName, string InsuranceNumber, string MobileNumber, IFormFile InsuranceCardImage, IFormFile PrescriptionImage) : IRequest<InsuranceApprovalSubmissionResponseDto>;
public sealed class SubmitInsuranceApprovalRequestCommandHandler(IInsuranceApprovalRequestService service) : IRequestHandler<SubmitInsuranceApprovalRequestCommand, InsuranceApprovalSubmissionResponseDto>
{
    public Task<InsuranceApprovalSubmissionResponseDto> Handle(SubmitInsuranceApprovalRequestCommand r, CancellationToken ct) =>
        service.SubmitAsync(r.PatientId, r.InsuredName, r.InsuranceNumber, r.MobileNumber, r.InsuranceCardImage, r.PrescriptionImage, ct);
}

public sealed record ListMyInsuranceApprovalRequestsQuery(string PatientId, int Page, int PageSize) : IRequest<PagedResult<InsuranceApprovalRequestDto>>;
public sealed class ListMyInsuranceApprovalRequestsQueryHandler(IInsuranceApprovalRequestService service) : IRequestHandler<ListMyInsuranceApprovalRequestsQuery, PagedResult<InsuranceApprovalRequestDto>>
{
    public Task<PagedResult<InsuranceApprovalRequestDto>> Handle(ListMyInsuranceApprovalRequestsQuery request, CancellationToken cancellationToken) =>
        service.ListMyAsync(request.PatientId, request.Page, request.PageSize, cancellationToken);
}

public sealed record GetMyInsuranceApprovalRequestQuery(string PatientId, int Id) : IRequest<InsuranceApprovalRequestDetailsDto>;
public sealed class GetMyInsuranceApprovalRequestQueryHandler(IInsuranceApprovalRequestService service) : IRequestHandler<GetMyInsuranceApprovalRequestQuery, InsuranceApprovalRequestDetailsDto>
{
    public Task<InsuranceApprovalRequestDetailsDto> Handle(GetMyInsuranceApprovalRequestQuery request, CancellationToken cancellationToken) =>
        service.GetMyByIdAsync(request.PatientId, request.Id, cancellationToken);
}

public sealed record ListInsuranceApprovalRequestsQuery(int Page, int PageSize, InsuranceApprovalRequestStatus? Status, string? Search) : IRequest<PagedResult<InsuranceApprovalRequestDto>>;
public sealed class ListInsuranceApprovalRequestsQueryHandler(IInsuranceApprovalRequestService service) : IRequestHandler<ListInsuranceApprovalRequestsQuery, PagedResult<InsuranceApprovalRequestDto>>
{
    public Task<PagedResult<InsuranceApprovalRequestDto>> Handle(ListInsuranceApprovalRequestsQuery request, CancellationToken cancellationToken) =>
        service.ListAsync(request.Page, request.PageSize, request.Status, request.Search, cancellationToken);
}

public sealed record GetInsuranceApprovalRequestQuery(int Id) : IRequest<InsuranceApprovalRequestDetailsDto>;
public sealed class GetInsuranceApprovalRequestQueryHandler(IInsuranceApprovalRequestService service) : IRequestHandler<GetInsuranceApprovalRequestQuery, InsuranceApprovalRequestDetailsDto>
{
    public Task<InsuranceApprovalRequestDetailsDto> Handle(GetInsuranceApprovalRequestQuery request, CancellationToken cancellationToken) =>
        service.GetByIdAsync(request.Id, cancellationToken);
}

public sealed record UpdateInsuranceApprovalRequestStatusCommand(int Id, InsuranceApprovalRequestStatus Status, string? Notes, string? RejectionReason) : IRequest<InsuranceApprovalRequestDetailsDto>;
public sealed class UpdateInsuranceApprovalRequestStatusCommandHandler(IInsuranceApprovalRequestService service) : IRequestHandler<UpdateInsuranceApprovalRequestStatusCommand, InsuranceApprovalRequestDetailsDto>
{
    public Task<InsuranceApprovalRequestDetailsDto> Handle(UpdateInsuranceApprovalRequestStatusCommand request, CancellationToken cancellationToken) =>
        service.UpdateStatusAsync(request.Id, request.Status, request.Notes, request.RejectionReason, cancellationToken);
}

public sealed record DeleteInsuranceApprovalRequestCommand(int Id) : IRequest;
public sealed class DeleteInsuranceApprovalRequestCommandHandler(IInsuranceApprovalRequestService service) : IRequestHandler<DeleteInsuranceApprovalRequestCommand>
{
    public Task Handle(DeleteInsuranceApprovalRequestCommand request, CancellationToken cancellationToken) =>
        service.DeleteAsync(request.Id, cancellationToken);
}
