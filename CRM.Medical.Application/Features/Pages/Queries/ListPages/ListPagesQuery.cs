using CRM.Medical.Application.Features.Pages.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Pages.Queries.ListPages;

public sealed record ListPagesQuery : IRequest<IReadOnlyList<PageListItemDto>>;
