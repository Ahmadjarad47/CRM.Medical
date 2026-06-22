using CRM.Medical.API.Contracts.Admin.Pages;
using CRM.Medical.Application.Features.Pages;
using CRM.Medical.Application.Features.Pages.Commands.CreatePage;
using CRM.Medical.Application.Features.Pages.Commands.DeletePage;
using CRM.Medical.Application.Features.Pages.Commands.UpdatePage;
using CRM.Medical.Application.Features.Pages.DTOs;
using CRM.Medical.Application.Features.Pages.Queries.GetPageById;
using CRM.Medical.Application.Features.Pages.Queries.ListPages;

namespace CRM.Medical.API.Controllers.Admin;

[Route("api/admin/pages")]
public sealed class PagesController(ISender mediator) : AdminBaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PageListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(CancellationToken ct) =>
        Ok(await mediator.Send(new ListPagesQuery(), ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PageDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetPageByIdQuery(id), ct));

    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(PageDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreatePageRequest request, CancellationToken ct)
    {
        var command = new CreatePageCommand(
            request.TemplateKey,
            request.ParentId,
            request.Order,
            request.PublishStatus,
            request.PublishScheduledAt,
            request.PublishedAt,
            request.IsVisibleInNav,
            request.IsActive,
            request.Translations.Select(MapTranslation).ToList(),
            request.ContentBlocks.Select(MapBlock).ToList(),
            request.ChangeNotes);

        var dto = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPatch("{id:int}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(PageDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePageRequest request, CancellationToken ct)
    {
        var command = new UpdatePageCommand(
            id,
            request.TemplateKey,
            request.ParentId,
            request.Order,
            request.PublishStatus,
            request.PublishScheduledAt,
            request.PublishedAt,
            request.IsVisibleInNav,
            request.IsActive,
            request.Translations.Select(MapTranslation).ToList(),
            request.ContentBlocks.Select(MapBlock).ToList(),
            request.ChangeNotes);

        return Ok(await mediator.Send(command, ct));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await mediator.Send(new DeletePageCommand(id), ct);
        return NoContent();
    }

    private static PageTranslationInput MapTranslation(PageTranslationRequest translation) =>
        new(
            translation.Language,
            translation.Title,
            translation.Slug,
            translation.MetaTitle,
            translation.MetaDescription,
            translation.MetaKeywords,
            translation.OpenGraphImageUrl,
            translation.CanonicalUrl,
            translation.BreadcrumbTitle);

    private static ContentBlockInput MapBlock(ContentBlockRequest block) =>
        new(
            block.BlockType,
            block.Order,
            block.CustomCssClass,
            block.CustomStyles,
            block.Animation,
            block.VisibilityRules,
            block.IsActive,
            block.Localizations.Select(MapLocalization).ToList());

    private static BlockLocalizationInput MapLocalization(BlockLocalizationRequest localization) =>
        new(
            localization.Language,
            localization.Heading,
            localization.Subheading,
            localization.Description,
            localization.ContentData,
            localization.MediaUrl,
            localization.MediaAltText,
            localization.ButtonText,
            localization.ButtonLink,
            localization.ButtonStyle);
}
