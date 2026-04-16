using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Features.Templates;

public interface ITemplateRepository
{
    Task<Template> AddAsync(Template entity, CancellationToken cancellationToken = default);

    Task<Template?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Template>> ListByUserIdAsync(string userId, CancellationToken cancellationToken = default);
}

