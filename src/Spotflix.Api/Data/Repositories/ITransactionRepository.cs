using Spotflix.Api.Models.Payments;

namespace Spotflix.Api.Data.Repositories;

public interface ITransactionRepository : IRepository<Transaction>
{
    Task<IReadOnlyList<Transaction>> GetCardHistoryAsync(Guid cardId, CancellationToken ct = default);
    Task<IReadOnlyList<Transaction>> GetRecentAuthorizedAsync(Guid cardId, DateTime since, CancellationToken ct = default);
}
