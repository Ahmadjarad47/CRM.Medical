using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.ExternalPatients.DTOs;
using CRM.Medical.Application.Features.ExternalPatients.Services;
using MediatR;

namespace CRM.Medical.Application.Features.ExternalPatients.CQRS;

public sealed class ListExternalPatientsQueryHandler(IExternalPatientService externalPatients)
    : IRequestHandler<ListExternalPatientsQuery, PagedResult<ExternalPatientDto>>
{
    public Task<PagedResult<ExternalPatientDto>> Handle(
        ListExternalPatientsQuery request,
        CancellationToken cancellationToken) =>
        externalPatients.ListAsync(request.Page, request.PageSize, request.Search, cancellationToken);
}
