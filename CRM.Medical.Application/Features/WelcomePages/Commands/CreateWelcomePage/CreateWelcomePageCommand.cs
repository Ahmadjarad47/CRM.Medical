using CRM.Medical.Application.Features.WelcomePages.DTOs;
using CRM.Medical.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CRM.Medical.Application.Features.WelcomePages.Commands.CreateWelcomePage;

public sealed record CreateWelcomePageCommand(
    string Name,
    string Description,
    AdMediaType MediaType,
    IFormFile Media,
    bool IsActive) : IRequest<WelcomePageDto>;
