using MediatR;

namespace CRM.Medical.Application.Features.ExternalPatients.CQRS;

public sealed record LinkExternalPatientToDirectPatientCommand(
    int ExternalPatientId,
    string DirectPatientUserId) : IRequest<Unit>;
