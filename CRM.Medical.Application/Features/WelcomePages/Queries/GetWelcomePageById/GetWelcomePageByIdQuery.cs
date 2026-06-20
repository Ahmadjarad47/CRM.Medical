using CRM.Medical.Application.Features.WelcomePages.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.WelcomePages.Queries.GetWelcomePageById;

public sealed record GetWelcomePageByIdQuery(int Id) : IRequest<WelcomePageDto>;
