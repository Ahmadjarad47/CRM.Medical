using System.Text.Json;
using CRM.Medical.Application.Features.Users.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Users.Commands.CreateUser;

public sealed record CreateUserCommand(
    string Email,
    string FullName,
    string Password,
    string? City,
    string? PhoneNumber,
    IReadOnlyList<string> Roles,
    JsonElement? ProfileMetadata) : IRequest<UserDetailDto>;
