using MediatR;

namespace CRM.Medical.Application.Features.Ads.Commands.DeleteAd;

public sealed record DeleteAdCommand(int Id) : IRequest;
