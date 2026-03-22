using FluentValidation;

namespace CRM.Medical.Application.Features.Auth.RefreshToken;

public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().MaximumLength(128);
        RuleFor(x => x.RefreshToken).NotEmpty().MaximumLength(2048);
    }
}
