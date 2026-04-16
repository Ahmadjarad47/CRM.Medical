using CRM.Medical.Application.Exceptions;
using FluentValidation;

namespace CRM.Medical.Application.Features.SlideCards.Commands.CreateSlideCard;

public sealed class CreateSlideCardCommandValidator : AbstractValidator<CreateSlideCardCommand>
{
    public CreateSlideCardCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(x => x.ImageBytes)
            .NotNull()
            .Must(b => b.Length > 0)
            .WithMessage("Image is required.");

        RuleFor(x => x.ImageContentType)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.ImageFileName)
            .NotEmpty()
            .MaximumLength(255);

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

