using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.MedicalTests.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.MedicalTests.Queries.ListMedicalTests;

public sealed class ListMedicalTestsQueryHandler(IMedicalTestRepository repository)
    : IRequestHandler<ListMedicalTestsQuery, PagedResult<MedicalTestDto>>
{
    public async Task<PagedResult<MedicalTestDto>> Handle(
        ListMedicalTestsQuery request,
        CancellationToken cancellationToken)
    {
        var (items, total) = await repository.ListAsync(
            request.Category,
            request.Status,
            request.Page,
            request.PageSize,
            cancellationToken);

        return new PagedResult<MedicalTestDto>
        {
            Items = items.Select(t => t.ToDto()).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = total
        };
    }
}
