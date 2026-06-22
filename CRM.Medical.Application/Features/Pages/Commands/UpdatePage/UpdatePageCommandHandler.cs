using CRM.Medical.Application.Abstractions;
using CRM.Medical.Application.Common.Time;
using CRM.Medical.Application.Exceptions;
using CRM.Medical.Application.Features.Pages.DTOs;
using CRM.Medical.Domain.Constants;
using CRM.Medical.Domain.Entities;
using MediatR;

namespace CRM.Medical.Application.Features.Pages.Commands.UpdatePage;

public sealed class UpdatePageCommandHandler(
    IPageRepository pages,
    IDateTimeProvider dateTimeProvider,
    ICurrentUserAccessor currentUser)
    : IRequestHandler<UpdatePageCommand, PageDto>
{
    public async Task<PageDto> Handle(UpdatePageCommand request, CancellationToken cancellationToken)
    {
        var entity = await pages.GetByIdWithDetailsForUpdateAsync(request.Id, cancellationToken)
            ?? throw new ApplicationNotFoundException($"Page '{request.Id}' was not found.");

        var now = dateTimeProvider.UtcNow;
        var templateKey = request.TemplateKey.Trim();
        var publishStatus = PageFeatureHelpers.NormalizeStatus(request.PublishStatus);

        if (await pages.TemplateKeyExistsAsync(templateKey, request.Id, cancellationToken))
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
            if (await pages.SlugExistsAsync(language, slug, request.Id, cancellationToken))
                throw new BusinessRuleException("Invalid page data", $"Slug '{slug}' already exists for language '{language}'.");
        }

        var (publishScheduledAt, publishedAt) = ResolvePublishingDates(
            publishStatus,
            request.PublishScheduledAt,
            request.PublishedAt,
            now);

        entity.TemplateKey = templateKey;
        entity.ParentId = request.ParentId;
        entity.Order = request.Order;
        entity.PublishStatus = publishStatus;
        entity.PublishScheduledAt = publishScheduledAt;
        entity.PublishedAt = publishedAt;
        entity.IsVisibleInNav = request.IsVisibleInNav;
        entity.IsActive = request.IsActive;
        entity.UpdatedByUserId = string.IsNullOrWhiteSpace(currentUser.UserId) ? entity.UpdatedByUserId : currentUser.UserId!.Trim();
        entity.UpdatedAt = now;

        entity.Translations.Clear();
        foreach (var translation in request.Translations.Select(MapTranslation))
            entity.Translations.Add(translation);

        entity.ContentBlocks.Clear();
        foreach (var block in request.ContentBlocks.Select(b => MapContentBlock(b, now)))
            entity.ContentBlocks.Add(block);

        var versionNumber = entity.Versions.Count == 0
            ? 1
            : entity.Versions.Max(v => v.VersionNumber) + 1;

        var versionCreatedBy = string.IsNullOrWhiteSpace(currentUser.UserId)
            ? entity.CreatedByUserId
            : currentUser.UserId!.Trim();

        entity.Versions.Add(new ContentVersion
        {
            VersionNumber = versionNumber,
            SnapshotData = PageFeatureHelpers.BuildSnapshot(entity),
            ChangeNotes = string.IsNullOrWhiteSpace(request.ChangeNotes) ? "Updated page content" : request.ChangeNotes.Trim(),
            CreatedByUserId = versionCreatedBy,
            CreatedAt = now
        });

        await pages.UpdateAsync(entity, cancellationToken);
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
