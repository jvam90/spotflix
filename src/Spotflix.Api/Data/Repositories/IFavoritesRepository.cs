using Spotflix.Api.Models.Social;

namespace Spotflix.Api.Data.Repositories;

public interface IFavoritesRepository
{
    Task<IReadOnlyList<FavoriteSong>> GetUserFavoriteSongsAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<FavoriteBand>> GetUserFavoriteBandsAsync(Guid userId, CancellationToken ct = default);
    Task<bool> IsSongFavoriteAsync(Guid userId, Guid songId, CancellationToken ct = default);
    Task<bool> IsBandFavoriteAsync(Guid userId, Guid bandId, CancellationToken ct = default);

    Task AddFavoriteSongAsync(Guid userId, Guid songId, CancellationToken ct = default);
    Task AddFavoriteBandAsync(Guid userId, Guid bandId, CancellationToken ct = default);

    Task RemoveFavoriteSongAsync(Guid userId, Guid songId, CancellationToken ct = default);
    Task RemoveFavoriteBandAsync(Guid userId, Guid bandId, CancellationToken ct = default);
}
