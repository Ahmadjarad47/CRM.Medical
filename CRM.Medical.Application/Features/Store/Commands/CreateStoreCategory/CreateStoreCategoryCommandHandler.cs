using CRM.Medical.Application.Features.Store.DTOs;
using CRM.Medical.Application.Features.Store.Services;
using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.CreateStoreCategory;

public sealed class CreateStoreCategoryCommandHandler(IStoreAdminService service)
    : IRequestHandler<CreateStoreCategoryCommand, ProductCategoryDto>
{
    public Task<ProductCategoryDto> Handle(CreateStoreCategoryCommand request, CancellationToken cancellationToken) =>
        service.CreateCategoryAsync(request.NameAr, request.NameEn, request.Description, request.ImageUrl, request.ParentCategoryId, request.DisplayOrder, request.IsActive, cancellationToken);
}
