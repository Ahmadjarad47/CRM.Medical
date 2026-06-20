using CRM.Medical.Application.Features.WelcomePages.DTOs;
using CRM.Medical.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CRM.Medical.Application.Features.WelcomePages.Commands.UpdateWelcomePage;

public sealed record UpdateWelcomePageCommand(
    int Id,
    string Name,
    string Description,
    AdMediaType MediaType,
    IFormFile? Media,
    bool IsActive) : IRequest<WelcomePageDto>;
