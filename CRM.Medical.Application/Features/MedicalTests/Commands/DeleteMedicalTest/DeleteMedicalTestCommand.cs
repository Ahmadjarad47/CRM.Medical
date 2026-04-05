using MediatR;

namespace CRM.Medical.Application.Features.MedicalTests.Commands.DeleteMedicalTest;

public sealed record DeleteMedicalTestCommand(int Id) : IRequest;
