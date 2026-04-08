using MediatR;

namespace CRM.Medical.Application.Features.TestRequests.Commands.DeleteTestRequest;

public sealed record DeleteTestRequestCommand(int Id) : IRequest;
