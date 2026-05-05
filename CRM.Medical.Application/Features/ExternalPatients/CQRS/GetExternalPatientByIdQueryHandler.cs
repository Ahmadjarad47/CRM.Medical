using CRM.Medical.Application.Features.ExternalPatients.DTOs;
using CRM.Medical.Application.Features.ExternalPatients.Services;
using MediatR;

namespace CRM.Medical.Application.Features.ExternalPatients.CQRS;

public sealed class GetExternalPatientByIdQueryHandler(IExternalPatientService externalPatients)
    : IRequestHandler<GetExternalPatientByIdQuery, ExternalPatientDto>
{
    public Task<ExternalPatientDto> Handle(
        GetExternalPatientByIdQuery request,
        CancellationToken cancellationToken) =>
        externalPatients.GetByIdAsync(request.Id, cancellationToken);
}
