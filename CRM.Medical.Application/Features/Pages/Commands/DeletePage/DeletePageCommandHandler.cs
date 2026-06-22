using CRM.Medical.Application.Exceptions;
using MediatR;

namespace CRM.Medical.Application.Features.Pages.Commands.DeletePage;

public sealed class DeletePageCommandHandler(IPageRepository pages) : IRequestHandler<DeletePageCommand>
{
    public async Task Handle(DeletePageCommand request, CancellationToken cancellationToken)
    {
        var entity = await pages.GetByIdWithDetailsForUpdateAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Page '{request.Id}' was not found.");

        await pages.DeleteAsync(entity, cancellationToken);
    }
}
