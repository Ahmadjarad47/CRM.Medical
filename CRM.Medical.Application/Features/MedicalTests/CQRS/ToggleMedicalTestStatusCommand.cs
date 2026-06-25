using CRM.Medical.Domain.Enums;
using MediatR;

namespace CRM.Medical.Application.Features.MedicalTests.CQRS;

public sealed record ToggleMedicalTestStatusCommand(int Id, MedicalTestStatus Status) : IRequest<Unit>;
