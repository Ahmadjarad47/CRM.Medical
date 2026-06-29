using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Pages.DTOs;
using CRM.Medical.Domain.Constants;
using CRM.Medical.Domain.Entities;
using MediatR;

namespace CRM.Medical.Application.Features.Pages.Commands.CreatePage;

public sealed class CreatePageCommandHandler(
    IPageRepository pages,
    IDateTimeProvider dateTimeProvider,
    ICurrentUserAccessor currentUser)
    : IRequestHandler<CreatePageCommand, PageDto>
{
    public async Task<PageDto> Handle(CreatePageCommand request, CancellationToken cancellationToken)
    {
        var now = dateTimeProvider.UtcNow;
        var templateKey = request.TemplateKey.Trim();
        var publishStatus = PageFeatureHelpers.NormalizeStatus(request.PublishStatus);

        if (await pages.TemplateKeyExistsAsync(templateKey, null, cancellationToken))
            throw new BusinessRuleException("Invalid page data", $"Template key '{templateKey}' already exists.");

        if (request.ParentId.HasValue)
        {
            var parentExists = await pages.GetByIdWithDetailsAsync(request.ParentId.Value, cancellationToken);
            if (parentExists is null)
                throw new ApplicationNotFoundException($"Parent page '{request.ParentId.Value}' was not found.");
        }

        foreach (var translation in request.Translations)
        {
            var language = PageFeatureHelpers.NormalizeLanguage(translation.Language);
            var slug = PageFeatureHelpers.NormalizeSlug(translation.Slug);
            if (await pages.SlugExistsAsync(language, slug, null, cancellationToken))
                throw new BusinessRuleException("Invalid page data", $"Slug '{slug}' already exists for language '{language}'.");
        }

        var (publishScheduledAt, publishedAt) = ResolvePublishingDates(
            publishStatus,
            request.PublishScheduledAt,
            request.PublishedAt,
            now);

        var createdByUserId = string.IsNullOrWhiteSpace(currentUser.UserId)
            ? "system"
            : currentUser.UserId!.Trim();

        var entity = new Page
        {
            TemplateKey = templateKey,
            ParentId = request.ParentId,
            Order = request.Order,
            PublishStatus = publishStatus,
            PublishScheduledAt = publishScheduledAt,
            PublishedAt = publishedAt,
            IsVisibleInNav = request.IsVisibleInNav,
            IsActive = request.IsActive,
            VisibleToRoles = PageFeatureHelpers.NormalizeVisibleToRoles(request.VisibleToRoles).ToList(),
            CreatedByUserId = createdByUserId,
            CreatedAt = now,
            Translations = request.Translations.Select(MapTranslation).ToList(),
            ContentBlocks = request.ContentBlocks.Select(b => MapContentBlock(b, now)).ToList()
        };

        entity.Versions.Add(new ContentVersion
        {
            VersionNumber = 1,
            SnapshotData = PageFeatureHelpers.BuildSnapshot(entity),
            ChangeNotes = string.IsNullOrWhiteSpace(request.ChangeNotes) ? "Initial version" : request.ChangeNotes.Trim(),
            CreatedByUserId = createdByUserId,
            CreatedAt = now
        });

        await pages.AddAsync(entity, cancellationToken);
        return entity.ToDto();
    }

    private static (DateTime? PublishScheduledAt, DateTime? PublishedAt) ResolvePublishingDates(
        string publishStatus,
        DateTime? publishScheduledAt,
        DateTime? publishedAt,
        DateTime now)
    {
        if (string.Equals(publishStatus, PagePublishStatuses.Published, StringComparison.OrdinalIgnoreCase))
            return (null, publishedAt ?? now);

        if (string.Equals(publishStatus, PagePublishStatuses.Scheduled, StringComparison.OrdinalIgnoreCase))
            return (publishScheduledAt, null);

        return (null, null);
    }

    private static PageTranslation MapTranslation(PageTranslationInput input) =>
        new()
        {
            Language = PageFeatureHelpers.NormalizeLanguage(input.Language),
            Title = input.Title.Trim(),
            Slug = PageFeatureHelpers.NormalizeSlug(input.Slug),
            MetaTitle = NormalizeOptional(input.MetaTitle),
            MetaDescription = NormalizeOptional(input.MetaDescription),
            MetaKeywords = NormalizeOptional(input.MetaKeywords),
            OpenGraphImageUrl = NormalizeOptional(input.OpenGraphImageUrl),
            CanonicalUrl = NormalizeOptional(input.CanonicalUrl),
            BreadcrumbTitle = NormalizeOptional(input.BreadcrumbTitle)
        };

    private static ContentBlock MapContentBlock(ContentBlockInput input, DateTime now) =>
        new()
        {
            BlockType = input.BlockType.Trim(),
            Order = input.Order,
            CustomCssClass = NormalizeOptional(input.CustomCssClass),
            CustomStyles = PageFeatureHelpers.ToJsonDocument(input.CustomStyles),
            Animation = NormalizeOptional(input.Animation),
            VisibilityRules = PageFeatureHelpers.ToJsonDocument(input.VisibilityRules),
            IsActive = input.IsActive,
            CreatedAt = now,
            Localizations = input.Localizations.Select(MapLocalization).ToList()
        };

    private static BlockLocalization MapLocalization(BlockLocalizationInput input) =>
        new()
        {
            Language = PageFeatureHelpers.NormalizeLanguage(input.Language),
            Heading = NormalizeOptional(input.Heading),
            Subheading = NormalizeOptional(input.Subheading),
            Description = NormalizeOptional(input.Description),
            ContentData = PageFeatureHelpers.ToJsonDocument(input.ContentData),
            MediaUrl = NormalizeOptional(input.MediaUrl),
            MediaAltText = NormalizeOptional(input.MediaAltText),
            ButtonText = NormalizeOptional(input.ButtonText),
            ButtonLink = NormalizeOptional(input.ButtonLink),
            ButtonStyle = NormalizeOptional(input.ButtonStyle)
        };

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
