using CRM.Medical.Domain.Entities;

namespace CRM.Medical.Application.Features.WelcomePages;

public interface IWelcomePageRepository
{
    Task<WelcomePage?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task AddAsync(WelcomePage entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(WelcomePage entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(WelcomePage entity, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WelcomePage>> ListAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WelcomePage>> ListActiveAsync(CancellationToken cancellationToken = default);
}
