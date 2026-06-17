using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.UpdateStoreCategory;

public sealed class UpdateStoreCategoryCommandHandler(IStoreAdminService service)
    : IRequestHandler<UpdateStoreCategoryCommand, ProductCategoryDto>
{
    public Task<ProductCategoryDto> Handle(UpdateStoreCategoryCommand request, CancellationToken cancellationToken) =>
        service.UpdateCategoryAsync(request.Id, request.NameAr, request.NameEn, request.Description, request.ImageUrl, request.ParentCategoryId, request.DisplayOrder, request.IsActive, cancellationToken);
}
