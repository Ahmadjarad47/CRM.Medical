using MediatR;

namespace CRM.Medical.Application.Features.Permissions.CQRS;

public sealed record DeleteAccessPolicyCommand(Guid Id) : IRequest<Unit>;
