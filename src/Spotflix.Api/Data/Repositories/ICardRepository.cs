using Spotflix.Api.Models.Payments;

namespace Spotflix.Api.Data.Repositories;

public interface ICardRepository : IRepository<Card>
{
    Task<IReadOnlyList<Card>> GetUserCardsAsync(Guid userId, CancellationToken ct = default);
    Task<Card?> GetByIdWithUserAsync(Guid id, CancellationToken ct = default);
}
