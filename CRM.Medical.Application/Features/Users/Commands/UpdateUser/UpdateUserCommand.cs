using System.Text.Json;
using CRM.Medical.Application.Features.Users.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Users.Commands.UpdateUser;

public sealed record UpdateUserCommand(
    string UserId,
    string FullName,
    string? City,
    string? PhoneNumber,
    JsonElement? ProfileMetadata) : IRequest<UserDetailDto>;
