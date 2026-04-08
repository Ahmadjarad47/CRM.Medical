using MediatR;

namespace CRM.Medical.Application.Features.MedicalTestResults.Commands.DeleteTestResult;

public sealed record DeleteTestResultCommand(int Id) : IRequest;
