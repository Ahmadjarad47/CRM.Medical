using MediatR;

namespace CRM.Medical.Application.Features.TestRequests.CQRS;

public sealed record DeleteTestRequestCommand(int Id) : IRequest<Unit>;
