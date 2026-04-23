using CRM.Medical.Application.Common.Json;
using CRM.Medical.Application.Common.Storage;
using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.MedicalTestResults;
using CRM.Medical.Application.Features.MedicalTestResults.DTOs;
using CRM.Medical.Application.Features.TestRequests;
using MediatR;

namespace CRM.Medical.Application.Features.MedicalTestResults.Commands.UpdateTestResult;

public sealed class UpdateTestResultCommandHandler(
    ITestResultRepository repository,
    IFileStorageService fileStorage,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<UpdateTestResultCommand, TestResultDto>
{
    public async Task<TestResultDto> Handle(
        UpdateTestResultCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Test result '{request.Id}' was not found.");

        entity.ResultData?.Dispose();
        entity.ResultData = ProfileMetadataMapper.ToJsonDocument(request.ResultData);

        entity.ResultDate = request.ResultDate;
        if (request.PdfFile is { Length: > 0 })
            entity.PdfUrl = await fileStorage.UploadPdfAsync(request.PdfFile, cancellationToken);
        entity.Status = request.Status;
        entity.UpdatedAt = dateTimeProvider.UtcNow;

        await repository.UpdateAsync(entity, cancellationToken);
        var reloaded = await repository.GetByIdAsync(entity.Id, cancellationToken);
        return reloaded!.ToDto();
    }
}
