using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.ServiceRequests.DTOs;
using CRM.Medical.Application.Features.ServiceRequests.Services;
using CRM.Medical.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CRM.Medical.Application.Features.ServiceRequests.CQRS;

public sealed record GetServiceRequestPageSettingQuery(ServiceRequestPageType PageType) : IRequest<ServiceRequestPageSettingDto>;
public sealed class GetServiceRequestPageSettingQueryHandler(IServiceRequestPageSettingsService service) : IRequestHandler<GetServiceRequestPageSettingQuery, ServiceRequestPageSettingDto>
{
    public Task<ServiceRequestPageSettingDto> Handle(GetServiceRequestPageSettingQuery request, CancellationToken cancellationToken) =>
        service.GetPublicByTypeAsync(request.PageType, cancellationToken);
}

public sealed record ListServiceRequestPageSettingsQuery : IRequest<IReadOnlyList<ServiceRequestPageSettingDto>>;
public sealed class ListServiceRequestPageSettingsQueryHandler(IServiceRequestPageSettingsService service) : IRequestHandler<ListServiceRequestPageSettingsQuery, IReadOnlyList<ServiceRequestPageSettingDto>>
{
    public Task<IReadOnlyList<ServiceRequestPageSettingDto>> Handle(ListServiceRequestPageSettingsQuery request, CancellationToken cancellationToken) =>
        service.ListAsync(cancellationToken);
}

public sealed record UpdateServiceRequestPageSettingCommand(int Id, string AnnouncementTextAr, string AnnouncementTextEn, string TitleAr, string TitleEn, string DescriptionAr, string DescriptionEn, bool IsActive) : IRequest<ServiceRequestPageSettingDto>;
public sealed class UpdateServiceRequestPageSettingCommandHandler(IServiceRequestPageSettingsService service) : IRequestHandler<UpdateServiceRequestPageSettingCommand, ServiceRequestPageSettingDto>
{
    public Task<ServiceRequestPageSettingDto> Handle(UpdateServiceRequestPageSettingCommand r, CancellationToken ct) =>
        service.UpdateAsync(r.Id, r.AnnouncementTextAr, r.AnnouncementTextEn, r.TitleAr, r.TitleEn, r.DescriptionAr, r.DescriptionEn, r.IsActive, ct);
}

public sealed record ListVacantJobsQuery(int Page, int PageSize, bool IncludeInactive) : IRequest<PagedResult<VacantJobDto>>;
public sealed class ListVacantJobsQueryHandler(IVacantJobsService service) : IRequestHandler<ListVacantJobsQuery, PagedResult<VacantJobDto>>
{
    public Task<PagedResult<VacantJobDto>> Handle(ListVacantJobsQuery request, CancellationToken cancellationToken) =>
        service.ListAsync(request.Page, request.PageSize, request.IncludeInactive, cancellationToken);
}

public sealed record GetVacantJobQuery(int Id, bool IncludeInactive) : IRequest<VacantJobDto>;
public sealed class GetVacantJobQueryHandler(IVacantJobsService service) : IRequestHandler<GetVacantJobQuery, VacantJobDto>
{
    public Task<VacantJobDto> Handle(GetVacantJobQuery request, CancellationToken cancellationToken) =>
        service.GetByIdAsync(request.Id, request.IncludeInactive, cancellationToken);
}

public sealed record SaveVacantJobCommand(int? Id, string TitleAr, string TitleEn, string? DescriptionAr, string? DescriptionEn, bool IsActive, int SortOrder) : IRequest<VacantJobDto>;
public sealed class SaveVacantJobCommandHandler(IVacantJobsService service) : IRequestHandler<SaveVacantJobCommand, VacantJobDto>
{
    public Task<VacantJobDto> Handle(SaveVacantJobCommand r, CancellationToken ct) =>
        r.Id is null
            ? service.CreateAsync(r.TitleAr, r.TitleEn, r.DescriptionAr, r.DescriptionEn, r.IsActive, r.SortOrder, ct)
            : service.UpdateAsync(r.Id.Value, r.TitleAr, r.TitleEn, r.DescriptionAr, r.DescriptionEn, r.IsActive, r.SortOrder, ct);
}

public sealed record DeleteVacantJobCommand(int Id) : IRequest;
public sealed class DeleteVacantJobCommandHandler(IVacantJobsService service) : IRequestHandler<DeleteVacantJobCommand>
{
    public Task Handle(DeleteVacantJobCommand request, CancellationToken cancellationToken) =>
        service.DeleteAsync(request.Id, cancellationToken);
}

public sealed record SubmitEmploymentApplicationCommand(string FullName, string ResidencePlace, string MobileNumber, string Email, string AcademicDegree, string PreviousExperience, int YearsOfExperience, string Skills, string? AdditionalCertificates, int VacantJobId, IFormFile CvFile) : IRequest<ServiceRequestSubmissionResponseDto>;
public sealed class SubmitEmploymentApplicationCommandHandler(IEmploymentApplicationService service) : IRequestHandler<SubmitEmploymentApplicationCommand, ServiceRequestSubmissionResponseDto>
{
    public Task<ServiceRequestSubmissionResponseDto> Handle(SubmitEmploymentApplicationCommand r, CancellationToken ct) =>
        service.SubmitAsync(r.FullName, r.ResidencePlace, r.MobileNumber, r.Email, r.AcademicDegree, r.PreviousExperience, r.YearsOfExperience, r.Skills, r.AdditionalCertificates, r.VacantJobId, r.CvFile, ct);
}

public sealed record ListEmploymentApplicationsQuery(int Page, int PageSize, string? Status) : IRequest<PagedResult<EmploymentApplicationRequestDto>>;
public sealed class ListEmploymentApplicationsQueryHandler(IEmploymentApplicationService service) : IRequestHandler<ListEmploymentApplicationsQuery, PagedResult<EmploymentApplicationRequestDto>>
{
    public Task<PagedResult<EmploymentApplicationRequestDto>> Handle(ListEmploymentApplicationsQuery request, CancellationToken cancellationToken) =>
        service.ListAsync(request.Page, request.PageSize, request.Status, cancellationToken);
}

public sealed record GetEmploymentApplicationQuery(int Id) : IRequest<EmploymentApplicationRequestDto>;
public sealed class GetEmploymentApplicationQueryHandler(IEmploymentApplicationService service) : IRequestHandler<GetEmploymentApplicationQuery, EmploymentApplicationRequestDto>
{
    public Task<EmploymentApplicationRequestDto> Handle(GetEmploymentApplicationQuery request, CancellationToken cancellationToken) =>
        service.GetByIdAsync(request.Id, cancellationToken);
}

public sealed record UpdateEmploymentApplicationStatusCommand(int Id, string Status, string? Notes) : IRequest<EmploymentApplicationRequestDto>;
public sealed class UpdateEmploymentApplicationStatusCommandHandler(IEmploymentApplicationService service) : IRequestHandler<UpdateEmploymentApplicationStatusCommand, EmploymentApplicationRequestDto>
{
    public Task<EmploymentApplicationRequestDto> Handle(UpdateEmploymentApplicationStatusCommand request, CancellationToken cancellationToken) =>
        service.UpdateStatusAsync(request.Id, request.Status, request.Notes, cancellationToken);
}

public sealed record DeleteEmploymentApplicationCommand(int Id) : IRequest;
public sealed class DeleteEmploymentApplicationCommandHandler(IEmploymentApplicationService service) : IRequestHandler<DeleteEmploymentApplicationCommand>
{
    public Task Handle(DeleteEmploymentApplicationCommand request, CancellationToken cancellationToken) =>
        service.DeleteAsync(request.Id, cancellationToken);
}

public sealed record SubmitClientJoinRequestCommand(string ManagerName, string LabName, string MobileNumber, string Email, string Address, string? AdditionalInfo) : IRequest<ServiceRequestSubmissionResponseDto>;
public sealed class SubmitClientJoinRequestCommandHandler(IClientJoinRequestService service) : IRequestHandler<SubmitClientJoinRequestCommand, ServiceRequestSubmissionResponseDto>
{
    public Task<ServiceRequestSubmissionResponseDto> Handle(SubmitClientJoinRequestCommand r, CancellationToken ct) =>
        service.SubmitAsync(r.ManagerName, r.LabName, r.MobileNumber, r.Email, r.Address, r.AdditionalInfo, ct);
}

public sealed record ListClientJoinRequestsQuery(int Page, int PageSize, string? Status) : IRequest<PagedResult<ClientJoinRequestDto>>;
public sealed class ListClientJoinRequestsQueryHandler(IClientJoinRequestService service) : IRequestHandler<ListClientJoinRequestsQuery, PagedResult<ClientJoinRequestDto>>
{
    public Task<PagedResult<ClientJoinRequestDto>> Handle(ListClientJoinRequestsQuery request, CancellationToken cancellationToken) =>
        service.ListAsync(request.Page, request.PageSize, request.Status, cancellationToken);
}

public sealed record GetClientJoinRequestQuery(int Id) : IRequest<ClientJoinRequestDto>;
public sealed class GetClientJoinRequestQueryHandler(IClientJoinRequestService service) : IRequestHandler<GetClientJoinRequestQuery, ClientJoinRequestDto>
{
    public Task<ClientJoinRequestDto> Handle(GetClientJoinRequestQuery request, CancellationToken cancellationToken) =>
        service.GetByIdAsync(request.Id, cancellationToken);
}

public sealed record UpdateClientJoinRequestStatusCommand(int Id, string Status, string? Notes) : IRequest<ClientJoinRequestDto>;
public sealed class UpdateClientJoinRequestStatusCommandHandler(IClientJoinRequestService service) : IRequestHandler<UpdateClientJoinRequestStatusCommand, ClientJoinRequestDto>
{
    public Task<ClientJoinRequestDto> Handle(UpdateClientJoinRequestStatusCommand request, CancellationToken cancellationToken) =>
        service.UpdateStatusAsync(request.Id, request.Status, request.Notes, cancellationToken);
}

public sealed record DeleteClientJoinRequestCommand(int Id) : IRequest;
public sealed class DeleteClientJoinRequestCommandHandler(IClientJoinRequestService service) : IRequestHandler<DeleteClientJoinRequestCommand>
{
    public Task Handle(DeleteClientJoinRequestCommand request, CancellationToken cancellationToken) =>
        service.DeleteAsync(request.Id, cancellationToken);
}

public sealed record SubmitContractServiceRequestCommand(ContractRequestType ContractType, string ResponsibleName, string? OrganizationName, int ExpectedSubscribersCount, string ContactNumber, string Email, string Address, ContractDuration ContractDuration, string? AdditionalInfo) : IRequest<ServiceRequestSubmissionResponseDto>;
public sealed class SubmitContractServiceRequestCommandHandler(IContractServiceRequestService service) : IRequestHandler<SubmitContractServiceRequestCommand, ServiceRequestSubmissionResponseDto>
{
    public Task<ServiceRequestSubmissionResponseDto> Handle(SubmitContractServiceRequestCommand r, CancellationToken ct) =>
        service.SubmitAsync(r.ContractType, r.ResponsibleName, r.OrganizationName, r.ExpectedSubscribersCount, r.ContactNumber, r.Email, r.Address, r.ContractDuration, r.AdditionalInfo, ct);
}

public sealed record ListContractServiceRequestsQuery(int Page, int PageSize, string? Status) : IRequest<PagedResult<ContractServiceRequestDto>>;
public sealed class ListContractServiceRequestsQueryHandler(IContractServiceRequestService service) : IRequestHandler<ListContractServiceRequestsQuery, PagedResult<ContractServiceRequestDto>>
{
    public Task<PagedResult<ContractServiceRequestDto>> Handle(ListContractServiceRequestsQuery request, CancellationToken cancellationToken) =>
        service.ListAsync(request.Page, request.PageSize, request.Status, cancellationToken);
}

public sealed record GetContractServiceRequestQuery(int Id) : IRequest<ContractServiceRequestDto>;
public sealed class GetContractServiceRequestQueryHandler(IContractServiceRequestService service) : IRequestHandler<GetContractServiceRequestQuery, ContractServiceRequestDto>
{
    public Task<ContractServiceRequestDto> Handle(GetContractServiceRequestQuery request, CancellationToken cancellationToken) =>
        service.GetByIdAsync(request.Id, cancellationToken);
}

public sealed record UpdateContractServiceRequestStatusCommand(int Id, string Status, string? Notes) : IRequest<ContractServiceRequestDto>;
public sealed class UpdateContractServiceRequestStatusCommandHandler(IContractServiceRequestService service) : IRequestHandler<UpdateContractServiceRequestStatusCommand, ContractServiceRequestDto>
{
    public Task<ContractServiceRequestDto> Handle(UpdateContractServiceRequestStatusCommand request, CancellationToken cancellationToken) =>
        service.UpdateStatusAsync(request.Id, request.Status, request.Notes, cancellationToken);
}

public sealed record DeleteContractServiceRequestCommand(int Id) : IRequest;
public sealed class DeleteContractServiceRequestCommandHandler(IContractServiceRequestService service) : IRequestHandler<DeleteContractServiceRequestCommand>
{
    public Task Handle(DeleteContractServiceRequestCommand request, CancellationToken cancellationToken) =>
        service.DeleteAsync(request.Id, cancellationToken);
}
