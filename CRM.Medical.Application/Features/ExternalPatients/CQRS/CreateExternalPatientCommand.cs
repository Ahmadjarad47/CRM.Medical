using CRM.Medical.Application.Features.ExternalPatients.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.ExternalPatients.CQRS;

public sealed record CreateExternalPatientCommand(
    string FullName,
    int? Age,
    string Gender,
    string PhoneNumber,
    string? ExternalId) : IRequest<ExternalPatientDto>;
