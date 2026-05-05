using MediatR;

namespace CRM.Medical.Application.Features.MedicalTests.CQRS;

public sealed record DeleteMedicalTestCommand(int Id) : IRequest<Unit>;
