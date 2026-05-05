using CRM.Medical.Application.Features.TestRequests.DTOs;
using CRM.Medical.Application.Features.TestRequests.Services;
using MediatR;

namespace CRM.Medical.Application.Features.TestRequests.CQRS;

public sealed class GetTestRequestByIdQueryHandler(ITestRequestService testRequests)
    : IRequestHandler<GetTestRequestByIdQuery, TestRequestDto>
{
    public Task<TestRequestDto> Handle(
        GetTestRequestByIdQuery request,
        CancellationToken cancellationToken) =>
        testRequests.GetByIdAsync(request.Id, cancellationToken);
}
