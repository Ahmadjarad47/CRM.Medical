using CRM.Medical.Application.Features.Pages.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Pages.Queries.ListWebsiteNavigationPages;

public sealed record ListWebsiteNavigationPagesQuery(string Language)
    : IRequest<IReadOnlyList<WebsiteNavigationPageDto>>;
