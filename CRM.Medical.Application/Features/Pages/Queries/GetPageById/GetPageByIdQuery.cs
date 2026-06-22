using CRM.Medical.Application.Features.Pages.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Pages.Queries.GetPageById;

public sealed record GetPageByIdQuery(int Id) : IRequest<PageDto>;
