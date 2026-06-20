using MediatR;

namespace CRM.Medical.Application.Features.WelcomePages.Commands.DeleteWelcomePage;

public sealed record DeleteWelcomePageCommand(int Id) : IRequest;
