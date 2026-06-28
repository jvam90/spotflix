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

    public async Task<Card?> GetByIdWithUserAsync(Guid id, CancellationToken ct = default)
    {
        return await DbSet
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }
}
