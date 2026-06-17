using MediatR;

namespace CRM.Medical.Application.Features.Banners.Commands.DeleteBanner;

public sealed record DeleteBannerCommand(int Id) : IRequest;
