using Microsoft.EntityFrameworkCore;
using Spotflix.Api.Models.Catalog;

namespace Spotflix.Api.Data.Repositories;

public class BandRepository : Repository<Band>, IBandRepository
{
    public BandRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Band?> GetWithAlbumsAsync(Guid id, CancellationToken ct = default)
    {
        return await DbSet.AsNoTracking()
            .Include(b => b.Albums)
            .FirstOrDefaultAsync(b => b.Id == id, ct);
    }
}
