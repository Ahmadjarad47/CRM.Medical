using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.DeleteStoreCategory;

public sealed record DeleteStoreCategoryCommand(int Id) : IRequest;
