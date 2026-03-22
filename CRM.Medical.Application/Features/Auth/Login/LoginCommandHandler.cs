using CRM.Medical.Application.Auth;
using CRM.Medical.Application.Mappings;
using FluentValidation;
using FluentValidation.Results;
using Mapster;
using MediatR;

namespace CRM.Medical.Application.Features.Auth.Login;

public sealed class LoginCommandHandler(IUserCredentialValidator credentialValidator, IJwtTokenGenerator tokenGenerator)
    : IRequestHandler<LoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();
        var validationResult = await credentialValidator.ValidateAsync(email, request.Password, cancellationToken);
        if (validationResult.User is null)
        {
            var message = validationResult.FailureReason switch
            {
                CredentialFailureReason.UserNotFound => "User is not registered.",
                CredentialFailureReason.InvalidPassword => "Wrong credentials: password is incorrect.",
                CredentialFailureReason.LockedOut => "User is locked.",
                CredentialFailureReason.EmailNotConfirmed => "Email is not confirmed.",
                _ => "Invalid credentials."
            };

            throw new ValidationException(
                [new ValidationFailure(nameof(LoginCommand.Email), message)]);
        }

        var (accessToken, expiresAtUtc) = tokenGenerator.CreateAccessToken(validationResult.User);
        var model = new LoginResponseMappingModel(
            accessToken,
            expiresAtUtc,
            validationResult.User.Email,
            validationResult.User.DisplayName);
        return model.Adapt<LoginResponse>();
    }
}
