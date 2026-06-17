using CRM.Medical.Application.Configuration.S3;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace CRM.Medical.Application.Features.Ads.Commands.UpdateAd;

public sealed class UpdateAdCommandValidator : AbstractValidator<UpdateAdCommand>
{
    public UpdateAdCommandValidator(IOptions<S3StorageSettings> s3Options)
    {
        var maxBytes = s3Options.Value.MaxAttachmentBytes;

        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(x => x.MediaType)
            .IsInEnum();

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
