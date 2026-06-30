using CRM.Medical.Application.Configuration.S3;
using CRM.Medical.Domain.Enums;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace CRM.Medical.Application.Features.Ads.Commands.CreateAd;

public sealed class CreateAdCommandValidator : AbstractValidator<CreateAdCommand>
{
    public CreateAdCommandValidator(IOptions<S3StorageSettings> s3Options)
    {
        var maxBytes = s3Options.Value.MaxAttachmentBytes;

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(x => x.AddressName)
            .NotEmpty()
            .MaximumLength(300);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90)
            .When(x => x.Latitude.HasValue);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180)
            .When(x => x.Longitude.HasValue);

        RuleFor(x => x)
            .Must(x => x.Latitude.HasValue == x.Longitude.HasValue)
            .WithMessage("Latitude and longitude must both be provided together.");

        RuleFor(x => x.MediaType)
            .IsInEnum();

        RuleFor(x => x.DisplayMode)
            .IsInEnum();

        RuleFor(x => x.Media)
            .NotNull()
            .Must(f => f.Length > 0)
            .WithMessage("Media file is required.");

        RuleFor(x => x.Media)
            .Must(f => f == null || f.Length <= maxBytes)
            .WithMessage($"Media file must not exceed {maxBytes} bytes.")
            .When(x => x.Media is { Length: > 0 });

        RuleFor(x => x)
            .Must(x => AdMediaValidation.MatchesMediaType(x.Media, x.MediaType))
            .WithMessage("Uploaded file does not match the selected media type.")
            .When(x => x.Media is { Length: > 0 });
    }
}
