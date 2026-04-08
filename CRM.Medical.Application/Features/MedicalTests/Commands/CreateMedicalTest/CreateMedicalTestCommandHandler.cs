using CRM.Medical.Application.Common.Json;
using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Features.MedicalTests.DTOs;
using CRM.Medical.Domain.Entities;
using MediatR;

namespace CRM.Medical.Application.Features.MedicalTests.Commands.CreateMedicalTest;

public sealed class CreateMedicalTestCommandHandler(
    IMedicalTestRepository repository,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<CreateMedicalTestCommand, MedicalTestDto>
{
    public async Task<MedicalTestDto> Handle(
        CreateMedicalTestCommand request,
        CancellationToken cancellationToken)
    {
        var now = dateTimeProvider.UtcNow;
        var entity = new MedicalTest
        {
            NameAr = request.NameAr,
            NameEn = request.NameEn,
            Price = request.Price,
            Category = request.Category,
            SampleType = request.SampleType,
            ParameterSchema = ProfileMetadataMapper.ToJsonDocument(request.ParameterSchema),
            Status = request.Status,
            CreatedByUserId = request.CreatedByUserId,
            CreatedAt = now
        };

        await repository.AddAsync(entity, cancellationToken);
        return entity.ToDto();
    }
}
