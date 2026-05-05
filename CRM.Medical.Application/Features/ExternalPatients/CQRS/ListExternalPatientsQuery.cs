using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.ExternalPatients.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.ExternalPatients.CQRS;

public sealed record ListExternalPatientsQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null) : IRequest<PagedResult<ExternalPatientDto>>;
