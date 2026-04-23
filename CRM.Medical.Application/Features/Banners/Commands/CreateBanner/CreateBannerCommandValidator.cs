using CRM.Medical.Application.Configuration.S3;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace CRM.Medical.Application.Features.Banners.Commands.CreateBanner;

public sealed class CreateBannerCommandValidator : AbstractValidator<CreateBannerCommand>
{
    public CreateBannerCommandValidator(IOptions<S3StorageSettings> s3Options)
    {
        var maxBytes = s3Options.Value.MaxAttachmentBytes;

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Type)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Media)
            .NotNull()
            .Must(f => f.Length > 0)
            .WithMessage("Media file is required.");

        RuleFor(x => x.Media)
            .Must(f => f == null || f.Length <= maxBytes)
            .WithMessage($"Media file must not exceed {maxBytes} bytes.")
            .When(x => x.Media is { Length: > 0 });

        RuleFor(x => x.InternalLink)
            .MaximumLength(2048);

        RuleFor(x => x.ExternalLink)
            .MaximumLength(2048);

        RuleFor(x => x.TargetType)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Location)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.StartDate)
            .Must(d => d != default)
            .WithMessage("StartDate is required.");

        RuleFor(x => x.EndDate)
            .Must(d => d != default)
            .WithMessage("EndDate is required.")
            .GreaterThan(x => x.StartDate)
            .WithMessage("EndDate must be greater than StartDate.");
    }
}

