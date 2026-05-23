using CRM.Medical.Application.Features.TestRequests.DTOs;
using CRM.Medical.Application.Features.TestRequests.Services;
using MediatR;

namespace CRM.Medical.Application.Features.TestRequests.CQRS;

public sealed class CreateTestRequestCommandHandler(ITestRequestService testRequests)
    : IRequestHandler<CreateTestRequestCommand, IReadOnlyList<TestRequestDto>>
{
    public Task<IReadOnlyList<TestRequestDto>> Handle(
        CreateTestRequestCommand request,
        CancellationToken cancellationToken) =>
        testRequests.CreateAsync(
            request.Items,
            cancellationToken);
}
