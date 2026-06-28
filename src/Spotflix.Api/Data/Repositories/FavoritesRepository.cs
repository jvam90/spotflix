using Microsoft.EntityFrameworkCore;
using Spotflix.Api.Models.Social;

namespace Spotflix.Api.Data.Repositories;

public class FavoritesRepository : IFavoritesRepository
{
    private readonly AppDbContext _db;

    public FavoritesRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<FavoriteSong>> GetUserFavoriteSongsAsync(Guid userId, CancellationToken ct = default)
    {
        return await _db.FavoriteSongs.AsNoTracking()
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<FavoriteBand>> GetUserFavoriteBandsAsync(Guid userId, CancellationToken ct = default)
    {
        return await _db.FavoriteBands.AsNoTracking()
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<bool> IsSongFavoriteAsync(Guid userId, Guid songId, CancellationToken ct = default)
    {
        return await _db.FavoriteSongs.AnyAsync(f => f.UserId == userId && f.SongId == songId, ct);
    }

    public async Task<bool> IsBandFavoriteAsync(Guid userId, Guid bandId, CancellationToken ct = default)
    {
        return await _db.FavoriteBands.AnyAsync(f => f.UserId == userId && f.BandId == bandId, ct);
    }

    public async Task AddFavoriteSongAsync(Guid userId, Guid songId, CancellationToken ct = default)
    {
        if (!await IsSongFavoriteAsync(userId, songId, ct))
        {
            _db.FavoriteSongs.Add(new FavoriteSong { UserId = userId, SongId = songId });
            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task AddFavoriteBandAsync(Guid userId, Guid bandId, CancellationToken ct = default)
    {
        if (!await IsBandFavoriteAsync(userId, bandId, ct))
        {
            _db.FavoriteBands.Add(new FavoriteBand { UserId = userId, BandId = bandId });
            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task RemoveFavoriteSongAsync(Guid userId, Guid songId, CancellationToken ct = default)
    {
        var fav = await _db.FavoriteSongs.FirstOrDefaultAsync(f => f.UserId == userId && f.SongId == songId, ct);
        if (fav is not null)
        {
            _db.FavoriteSongs.Remove(fav);
            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task RemoveFavoriteBandAsync(Guid userId, Guid bandId, CancellationToken ct = default)
    {
        var fav = await _db.FavoriteBands.FirstOrDefaultAsync(f => f.UserId == userId && f.BandId == bandId, ct);
        if (fav is not null)
        {
            _db.FavoriteBands.Remove(fav);
            await _db.SaveChangesAsync(ct);
        }
    }
}
