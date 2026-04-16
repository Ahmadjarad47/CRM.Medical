using FluentValidation;

namespace CRM.Medical.Application.Features.Banners.Commands.CreateBanner;

public sealed class CreateBannerCommandValidator : AbstractValidator<CreateBannerCommand>
{
    public CreateBannerCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Type)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.MediaBytes)
            .NotNull()
            .Must(b => b.Length > 0)
            .WithMessage("Media file is required.");

        RuleFor(x => x.MediaContentType)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.MediaFileName)
            .NotEmpty()
            .MaximumLength(255);

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
            .WithMessage("EndDate must be after StartDate.");
    }
}

