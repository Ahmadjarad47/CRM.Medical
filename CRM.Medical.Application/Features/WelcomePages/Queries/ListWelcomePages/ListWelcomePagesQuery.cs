using CRM.Medical.Application.Features.WelcomePages.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.WelcomePages.Queries.ListWelcomePages;

public sealed record ListWelcomePagesQuery : IRequest<IReadOnlyList<WelcomePageDto>>;
