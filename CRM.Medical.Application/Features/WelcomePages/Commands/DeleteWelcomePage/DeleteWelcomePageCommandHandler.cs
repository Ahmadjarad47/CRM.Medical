using CRM.Medical.Application.Exceptions;
using MediatR;

namespace CRM.Medical.Application.Features.WelcomePages.Commands.DeleteWelcomePage;

public sealed class DeleteWelcomePageCommandHandler(IWelcomePageRepository welcomePages)
    : IRequestHandler<DeleteWelcomePageCommand>
{
    public async Task Handle(DeleteWelcomePageCommand request, CancellationToken cancellationToken)
    {
        var entity = await welcomePages.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Welcome page '{request.Id}' was not found.");

        await welcomePages.DeleteAsync(entity, cancellationToken);
    }
}
