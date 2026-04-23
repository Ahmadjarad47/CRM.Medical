using CRM.Medical.Application.Configuration.S3;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace CRM.Medical.Application.Features.SlideCards.Commands.CreateSlideCard;

public sealed class CreateSlideCardCommandValidator : AbstractValidator<CreateSlideCardCommand>
{
    public CreateSlideCardCommandValidator(IOptions<S3StorageSettings> s3Options)
    {
        var maxBytes = s3Options.Value.MaxAttachmentBytes;

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(x => x.Image)
            .NotNull()
            .Must(f => f.Length > 0)
            .WithMessage("Image is required.");

        RuleFor(x => x.Image)
            .Must(f => f == null || f.Length <= maxBytes)
            .WithMessage($"Image must not exceed {maxBytes} bytes.")
            .When(x => x.Image is { Length: > 0 });

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Discount)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Badge)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.DetailPageLink)
            .NotEmpty()
            .MaximumLength(2048);

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.ExpiryDate)
            .Must(d => d != default)
            .WithMessage("ExpiryDate is required.");
    }
}

