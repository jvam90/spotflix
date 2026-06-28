using Microsoft.EntityFrameworkCore;
using Spotflix.Api.Models;

namespace Spotflix.Api.Data.Repositories;

public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<RefreshToken?> GetWithUserAsync(string token, CancellationToken ct = default)
    {
        return await DbSet.AsNoTracking()
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token, ct);
    }
}
