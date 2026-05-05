using CRM.Medical.Application.Features.MedicalTests.DTOs;
using CRM.Medical.Application.Features.MedicalTests.Services;
using MediatR;

namespace CRM.Medical.Application.Features.MedicalTests.CQRS;

public sealed class CreateMedicalTestCommandHandler(IMedicalTestService medicalTests)
    : IRequestHandler<CreateMedicalTestCommand, MedicalTestDto>
{
    public Task<MedicalTestDto> Handle(
        CreateMedicalTestCommand request,
        CancellationToken cancellationToken) =>
        medicalTests.CreateAsync(
            request.NameAr,
            request.NameEn,
            request.Price,
            request.Category,
            request.SampleType,
            request.ParameterSchema,
            request.Status,
            cancellationToken);
}
