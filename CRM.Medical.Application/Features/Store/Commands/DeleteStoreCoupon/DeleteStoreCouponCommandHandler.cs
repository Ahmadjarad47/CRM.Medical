using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.DeleteStoreCoupon;

public sealed class DeleteStoreCouponCommandHandler(IStoreAdminService service)
    : IRequestHandler<DeleteStoreCouponCommand>
{
    public Task Handle(DeleteStoreCouponCommand request, CancellationToken cancellationToken) =>
        service.DeleteCouponAsync(request.Id, cancellationToken);
}
