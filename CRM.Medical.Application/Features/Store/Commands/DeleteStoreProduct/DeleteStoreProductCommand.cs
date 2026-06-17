using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.DeleteStoreProduct;

public sealed record DeleteStoreProductCommand(int Id) : IRequest;
