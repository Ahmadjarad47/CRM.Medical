using CRM.Medical.Application.Features.Pages.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Pages.Queries.GetWebsitePageBySlug;

public sealed record GetWebsitePageBySlugQuery(string Slug, string Language) : IRequest<WebsitePageDto>;
