using System.ComponentModel.DataAnnotations;
using CRM.Medical.Application.Auth;
using MediatR;

namespace CRM.Medical.Application.Features.Auth.Login;

/// <summary>
/// Login request body. Rules mirror <see cref="LoginCommandValidator"/>; data annotations help OpenAPI describe required parameters.
/// </summary>
public sealed record LoginCommand(
    [property: Required(ErrorMessage = "Email is required.")]
    [property: EmailAddress(ErrorMessage = "Email must be a valid address.")]
    [property: MaxLength(256)]
    string Email,

    [property: Required(ErrorMessage = "Password is required.")]
    [property: MinLength(1, ErrorMessage = "Password is required.")]
    [property: MaxLength(512, ErrorMessage = "Password is too long.")]
    string Password)
    : IRequest<LoginResponse?>;