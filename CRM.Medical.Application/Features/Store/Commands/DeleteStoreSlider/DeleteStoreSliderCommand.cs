using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.DeleteStoreSlider;

public sealed record DeleteStoreSliderCommand(int Id) : IRequest;
