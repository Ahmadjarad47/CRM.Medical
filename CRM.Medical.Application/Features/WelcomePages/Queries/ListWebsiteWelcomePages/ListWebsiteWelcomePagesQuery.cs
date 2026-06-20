using CRM.Medical.Application.Features.WelcomePages.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.WelcomePages.Queries.ListWebsiteWelcomePages;

public sealed record ListWebsiteWelcomePagesQuery : IRequest<IReadOnlyList<WelcomePageDto>>;
