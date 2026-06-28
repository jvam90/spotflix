using Microsoft.EntityFrameworkCore;
using Spotflix.Api.Models.Catalog;

namespace Spotflix.Api.Data.Repositories;

public class SearchRepository : ISearchRepository
{
    private readonly AppDbContext _db;

    public SearchRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Band>> SearchBandsAsync(string term, int limit, CancellationToken ct = default)
    {
        var searchTerm = $"%{term.Trim()}%";
        limit = Math.Clamp(limit, 1, 50);

        return await _db.Bands.AsNoTracking()
            .Include(b => b.Albums)
            .Where(b => EF.Functions.ILike(b.Name, searchTerm))
            .OrderBy(b => b.Name)
            .Take(limit)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Song>> SearchSongsAsync(string term, int limit, CancellationToken ct = default)
    {
        var searchTerm = $"%{term.Trim()}%";
        limit = Math.Clamp(limit, 1, 50);

        return await _db.Songs.AsNoTracking()
            .Include(s => s.Album).ThenInclude(a => a.Band)
            .Where(s => EF.Functions.ILike(s.Title, searchTerm))
            .OrderBy(s => s.Title)
            .Take(limit)
            .ToListAsync(ct);
    }
}
