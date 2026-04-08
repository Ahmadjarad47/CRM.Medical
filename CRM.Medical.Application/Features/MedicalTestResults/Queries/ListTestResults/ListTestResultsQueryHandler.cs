using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.MedicalTestResults;
using CRM.Medical.Application.Features.MedicalTestResults.DTOs;
using CRM.Medical.Application.Features.TestRequests;
using MediatR;

namespace CRM.Medical.Application.Features.MedicalTestResults.Queries.ListTestResults;

public sealed class ListTestResultsQueryHandler(ITestResultRepository repository)
    : IRequestHandler<ListTestResultsQuery, PagedResult<TestResultDto>>
{
    public async Task<PagedResult<TestResultDto>> Handle(
        ListTestResultsQuery request,
        CancellationToken cancellationToken)
    {
        var (items, total) = await repository.ListAsync(
            request.TestRequestId,
            request.Status,
            request.Page,
            request.PageSize,
            cancellationToken);

        return new PagedResult<TestResultDto>
        {
            Items = items.Select(r => r.ToDto()).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = total
        };
    }
}
