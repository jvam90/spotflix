using Microsoft.EntityFrameworkCore;
using Spotflix.Api.Models.Payments;

namespace Spotflix.Api.Data.Repositories;

public class TransactionRepository : Repository<Transaction>, ITransactionRepository
{
    public TransactionRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<Transaction>> GetCardHistoryAsync(Guid cardId, CancellationToken ct = default)
    {
        return await DbSet.AsNoTracking()
            .Where(t => t.CardId == cardId)
            .OrderByDescending(t => t.OccurredAt)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Transaction>> GetRecentAuthorizedAsync(Guid cardId, DateTime since, CancellationToken ct = default)
    {
        return await DbSet.AsNoTracking()
            .Where(t => t.CardId == cardId
                        && t.Status == TransactionStatus.Authorized
                        && t.OccurredAt > since)
            .ToListAsync(ct);
    }
}
