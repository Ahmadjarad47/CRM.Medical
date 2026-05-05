using CRM.Medical.Application.Features.ExternalPatients.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.ExternalPatients.CQRS;

public sealed record GetExternalPatientByIdQuery(int Id) : IRequest<ExternalPatientDto>;
