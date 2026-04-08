using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.TestRequests.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.TestRequests.Queries.GetTestRequestById;

public sealed class GetTestRequestByIdQueryHandler(ITestRequestRepository repository)
    : IRequestHandler<GetTestRequestByIdQuery, TestRequestDto>
{
    public async Task<TestRequestDto> Handle(
        GetTestRequestByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Test request '{request.Id}' was not found.");

        return entity.ToDto();
    }
}
