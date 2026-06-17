using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.RemoveStoreCartItem;

public sealed record RemoveStoreCartItemCommand(string LabClientId, int ItemId) : IRequest;
