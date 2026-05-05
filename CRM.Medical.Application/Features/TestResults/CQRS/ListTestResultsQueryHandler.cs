using CRM.Medical.Application.Common.Responses;
using CRM.Medical.Application.Features.TestResults.DTOs;
using CRM.Medical.Application.Features.TestResults.Services;
using MediatR;

namespace CRM.Medical.Application.Features.TestResults.CQRS;

public sealed class ListTestResultsQueryHandler(ITestResultService testResults)
    : IRequestHandler<ListTestResultsQuery, PagedResult<TestResultDto>>
{
    public Task<PagedResult<TestResultDto>> Handle(
        ListTestResultsQuery request,
        CancellationToken cancellationToken) =>
        testResults.ListAsync(request.Page, request.PageSize, request.Search, request.TestRequestId, cancellationToken);
}
