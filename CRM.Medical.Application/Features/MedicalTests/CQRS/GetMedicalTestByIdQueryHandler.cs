using CRM.Medical.Application.Features.MedicalTests.DTOs;
using CRM.Medical.Application.Features.MedicalTests.Services;
using MediatR;

namespace CRM.Medical.Application.Features.MedicalTests.CQRS;

public sealed class GetMedicalTestByIdQueryHandler(IMedicalTestService medicalTests)
    : IRequestHandler<GetMedicalTestByIdQuery, MedicalTestDto>
{
    public Task<MedicalTestDto> Handle(
        GetMedicalTestByIdQuery request,
        CancellationToken cancellationToken) =>
        medicalTests.GetByIdAsync(request.Id, cancellationToken);
}
