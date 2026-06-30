using CRM.Medical.Application.Features.Users.Constants;
using CRM.Medical.Domain.Constants;
using FluentValidation;

namespace CRM.Medical.Application.Features.Pages.Commands.CreatePage;

public sealed class CreatePageCommandValidator : AbstractValidator<CreatePageCommand>
{
    public CreatePageCommandValidator()
    {
        RuleFor(x => x.TemplateKey)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(x => x.ParentId)
            .GreaterThan(0)
            .When(x => x.ParentId.HasValue);

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.PublishStatus)
            .NotEmpty()
            .Must(status => PagePublishStatuses.All.Contains(
                PageFeatureHelpers.NormalizeStatus(status),
                StringComparer.OrdinalIgnoreCase))
            .WithMessage("PublishStatus must be one of: Draft, Scheduled, Published, Archived.");

        RuleFor(x => x.PublishScheduledAt)
            .NotNull()
            .When(x => string.Equals(
                PageFeatureHelpers.NormalizeStatus(x.PublishStatus),
                PagePublishStatuses.Scheduled,
                StringComparison.OrdinalIgnoreCase))
            .WithMessage("PublishScheduledAt is required when PublishStatus is Scheduled.");

        RuleFor(x => x.VisibleToRoles)
            .Must(roles => roles is null || roles.All(IsKnownRole))
            .WithMessage($"VisibleToRoles must contain only known roles: {string.Join(", ", UserRoles.All)}.");

        RuleFor(x => x.Translations)
            .NotNull()
            .Must(t => t.Count > 0)
            .WithMessage("At least one page translation is required.");

        RuleForEach(x => x.Translations)
            .ChildRules(translation =>
            {
                translation.RuleFor(t => t.Language).NotEmpty().MaximumLength(10);
                translation.RuleFor(t => t.Title).NotEmpty().MaximumLength(300);
                translation.RuleFor(t => t.Slug).NotEmpty().MaximumLength(300);
                translation.RuleFor(t => t.MetaTitle).MaximumLength(300);
                translation.RuleFor(t => t.MetaDescription).MaximumLength(1000);
                translation.RuleFor(t => t.MetaKeywords).MaximumLength(1000);
                translation.RuleFor(t => t.OpenGraphImageUrl).MaximumLength(2048);
                translation.RuleFor(t => t.CanonicalUrl).MaximumLength(2048);
                translation.RuleFor(t => t.BreadcrumbTitle).MaximumLength(300);
            });

        RuleFor(x => x.Translations)
            .Must(HaveUniqueTranslationLanguages)
            .WithMessage("Translations must use unique languages.");

        RuleFor(x => x.Translations)
            .Must(HaveUniqueTranslationSlugsPerLanguage)
            .WithMessage("Translations must use unique slug values per language.");

        RuleForEach(x => x.ContentBlocks)
            .ChildRules(block =>
            {
                block.RuleFor(b => b.BlockType).NotEmpty().MaximumLength(100);
                block.RuleFor(b => b.Order).GreaterThanOrEqualTo(0);
                block.RuleFor(b => b.CustomCssClass).MaximumLength(200);
                block.RuleFor(b => b.Animation).MaximumLength(100);

                block.RuleForEach(b => b.Localizations)
                    .ChildRules(localization =>
                    {
                        localization.RuleFor(l => l.Language).NotEmpty().MaximumLength(10);
                        localization.RuleFor(l => l.Heading).MaximumLength(300);
                        localization.RuleFor(l => l.Subheading).MaximumLength(600);
                        localization.RuleFor(l => l.Description).MaximumLength(4000);
                        localization.RuleFor(l => l.MediaUrl).MaximumLength(2048);
                        localization.RuleFor(l => l.MediaAltText).MaximumLength(500);
                        localization.RuleFor(l => l.ButtonText).MaximumLength(300);
                        localization.RuleFor(l => l.ButtonLink).MaximumLength(2048);
                        localization.RuleFor(l => l.ButtonStyle).MaximumLength(100);
                    });

                block.RuleFor(b => b.Localizations)
                    .Must(HaveUniqueLocalizationLanguages)
                    .WithMessage("Each content block localization must use unique languages.");
            });
    }

    private static bool HaveUniqueTranslationLanguages(IReadOnlyList<PageTranslationInput> translations)
    {
        var distinct = translations
            .Select(t => PageFeatureHelpers.NormalizeLanguage(t.Language))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();

        return distinct == translations.Count;
    }

    private static bool HaveUniqueTranslationSlugsPerLanguage(IReadOnlyList<PageTranslationInput> translations)
    {
        var distinct = translations
            .Select(t => $"{PageFeatureHelpers.NormalizeLanguage(t.Language)}::{PageFeatureHelpers.NormalizeSlug(t.Slug)}")
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();

        return distinct == translations.Count;
    }

    private static bool HaveUniqueLocalizationLanguages(IReadOnlyList<BlockLocalizationInput> localizations)
    {
        var distinct = localizations
            .Select(l => PageFeatureHelpers.NormalizeLanguage(l.Language))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();

        return distinct == localizations.Count;
    }

    private static bool IsKnownRole(string role) =>
        UserRoles.All.Contains(role.Trim(), StringComparer.OrdinalIgnoreCase);
}
