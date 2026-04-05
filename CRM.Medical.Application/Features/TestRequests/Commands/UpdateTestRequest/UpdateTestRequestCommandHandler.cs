using CRM.Medical.Application.Common.Json;
using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.TestRequests.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.TestRequests.Commands.UpdateTestRequest;

public sealed class UpdateTestRequestCommandHandler(
    ITestRequestRepository repository,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<UpdateTestRequestCommand, TestRequestDto>
{
    public async Task<TestRequestDto> Handle(
        UpdateTestRequestCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Test request '{request.Id}' was not found.");

        entity.Metadata?.Dispose();
        entity.Metadata = ProfileMetadataMapper.ToJsonDocument(request.Metadata);

        entity.RequestDate = request.RequestDate;
        entity.Status = request.Status;
        entity.TotalAmount = request.TotalAmount;
        entity.Notes = request.Notes;
        entity.UpdatedAt = dateTimeProvider.UtcNow;

        await repository.UpdateAsync(entity, cancellationToken);
        var reloaded = await repository.GetByIdAsync(entity.Id, cancellationToken);
        return reloaded!.ToDto();
    }
}
