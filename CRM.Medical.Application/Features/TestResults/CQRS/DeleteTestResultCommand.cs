using MediatR;

namespace CRM.Medical.Application.Features.TestResults.CQRS;

public sealed record DeleteTestResultCommand(int Id) : IRequest<Unit>;
