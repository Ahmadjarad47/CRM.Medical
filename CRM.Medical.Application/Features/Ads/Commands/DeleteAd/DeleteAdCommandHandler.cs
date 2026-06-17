using CRM.Medical.Application.Exceptions;
using MediatR;

namespace CRM.Medical.Application.Features.Ads.Commands.DeleteAd;

public sealed class DeleteAdCommandHandler(IAdRepository ads)
    : IRequestHandler<DeleteAdCommand>
{
    public async Task Handle(DeleteAdCommand request, CancellationToken cancellationToken)
    {
        var entity = await ads.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Ad '{request.Id}' was not found.");

        await ads.DeleteAsync(entity, cancellationToken);
    }
}
