using MediatR;

namespace CRM.Medical.Application.Features.Store.Commands.DeleteStoreBanner;

public sealed record DeleteStoreBannerCommand(int Id) : IRequest;
