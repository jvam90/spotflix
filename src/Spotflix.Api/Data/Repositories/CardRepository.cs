using Microsoft.EntityFrameworkCore;
using Spotflix.Api.Models.Payments;

namespace Spotflix.Api.Data.Repositories;

public class CardRepository : Repository<Card>, ICardRepository
{
    public CardRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<Card>> GetUserCardsAsync(Guid userId, CancellationToken ct = default)
    {
        return await DbSet.AsNoTracking()
            .Where(c => c.UserId == userId)
            .ToListAsync(ct);
    }
}
