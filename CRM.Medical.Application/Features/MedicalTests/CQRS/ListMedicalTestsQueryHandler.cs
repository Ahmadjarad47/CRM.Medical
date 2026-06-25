using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.MedicalTests.DTOs;
using CRM.Medical.Application.Features.MedicalTests.Services;
using MediatR;

namespace CRM.Medical.Application.Features.MedicalTests.CQRS;

public sealed class ListMedicalTestsQueryHandler(IMedicalTestService medicalTests)
    : IRequestHandler<ListMedicalTestsQuery, PagedResult<MedicalTestDto>>
{
    public Task<PagedResult<MedicalTestDto>> Handle(
        ListMedicalTestsQuery request,
        CancellationToken cancellationToken) =>
        medicalTests.ListAsync(request.Page, request.PageSize, request.Search, request.CategoryMedicalId, cancellationToken);
}
