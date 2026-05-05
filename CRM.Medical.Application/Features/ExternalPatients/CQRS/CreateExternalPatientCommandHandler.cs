using CRM.Medical.Application.Features.ExternalPatients.DTOs;
using CRM.Medical.Application.Features.ExternalPatients.Services;
using MediatR;

namespace CRM.Medical.Application.Features.ExternalPatients.CQRS;

public sealed class CreateExternalPatientCommandHandler(IExternalPatientService externalPatients)
    : IRequestHandler<CreateExternalPatientCommand, ExternalPatientDto>
{
    public Task<ExternalPatientDto> Handle(
        CreateExternalPatientCommand request,
        CancellationToken cancellationToken) =>
        externalPatients.CreateAsync(
            request.FullName,
            request.Age,
            request.Gender,
            request.PhoneNumber,
            request.ExternalId,
            cancellationToken);
}
