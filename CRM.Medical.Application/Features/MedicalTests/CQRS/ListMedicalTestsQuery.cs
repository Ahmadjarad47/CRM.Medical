using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.MedicalTests.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.MedicalTests.CQRS;

public sealed record ListMedicalTestsQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    int? CategoryMedicalId = null) : IRequest<PagedResult<MedicalTestDto>>;
