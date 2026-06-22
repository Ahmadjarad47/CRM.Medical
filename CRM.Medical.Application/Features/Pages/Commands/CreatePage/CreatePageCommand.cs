using CRM.Medical.Application.Features.Pages.DTOs;
using MediatR;

namespace CRM.Medical.Application.Features.Pages.Commands.CreatePage;

public sealed record CreatePageCommand(
    string TemplateKey,
    int? ParentId,
    int Order,
    string PublishStatus,
    DateTime? PublishScheduledAt,
    DateTime? PublishedAt,
    bool IsVisibleInNav,
    bool IsActive,
    IReadOnlyList<PageTranslationInput> Translations,
    IReadOnlyList<ContentBlockInput> ContentBlocks,
    string? ChangeNotes) : IRequest<PageDto>;
