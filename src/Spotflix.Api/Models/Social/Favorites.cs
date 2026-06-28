using Spotflix.Api.Models.Catalog;

namespace Spotflix.Api.Models.Social;

/// <summary>Música favoritada por um usuário (chave composta UserId+SongId).</summary>
public class FavoriteSong
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public Guid SongId { get; set; }
    public Song Song { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>Banda favoritada por um usuário (chave composta UserId+BandId).</summary>
public class FavoriteBand
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public Guid BandId { get; set; }
    public Band Band { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
