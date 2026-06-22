using MediatR;

namespace CRM.Medical.Application.Features.Pages.Commands.DeletePage;

public sealed record DeletePageCommand(int Id) : IRequest;
