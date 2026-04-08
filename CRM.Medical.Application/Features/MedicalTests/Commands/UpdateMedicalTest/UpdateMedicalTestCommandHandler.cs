using CRM.Medical.Application.Common.Json;
using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.MedicalTests.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.MedicalTests.Commands.UpdateMedicalTest;

public sealed class UpdateMedicalTestCommandHandler(
    IMedicalTestRepository repository,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<UpdateMedicalTestCommand, MedicalTestDto>
{
    public async Task<MedicalTestDto> Handle(
        UpdateMedicalTestCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Medical test '{request.Id}' was not found.");

        entity.ParameterSchema?.Dispose();
        entity.ParameterSchema = ProfileMetadataMapper.ToJsonDocument(request.ParameterSchema);

        entity.NameAr = request.NameAr;
        entity.NameEn = request.NameEn;
        entity.Price = request.Price;
        entity.Category = request.Category;
        entity.SampleType = request.SampleType;
        entity.Status = request.Status;
        entity.UpdatedAt = dateTimeProvider.UtcNow;

        await repository.UpdateAsync(entity, cancellationToken);
        return entity.ToDto();
    }
}
