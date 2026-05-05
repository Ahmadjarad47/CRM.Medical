using CRM.Medical.Application.Features.ExternalPatients.Services;
using MediatR;

namespace CRM.Medical.Application.Features.ExternalPatients.CQRS;

public sealed class LinkExternalPatientToDirectPatientCommandHandler(IExternalPatientService externalPatients)
    : IRequestHandler<LinkExternalPatientToDirectPatientCommand, Unit>
{
    public async Task<Unit> Handle(
        LinkExternalPatientToDirectPatientCommand request,
        CancellationToken cancellationToken)
    {
        await externalPatients.LinkToDirectPatientAsync(
            request.ExternalPatientId,
            request.DirectPatientUserId,
            cancellationToken);
        return Unit.Value;
    }
}
