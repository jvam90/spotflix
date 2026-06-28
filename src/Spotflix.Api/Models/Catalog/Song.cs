namespace Spotflix.Api.Models.Catalog;

/// <summary>Música (faixa) de um álbum.</summary>
public class Song
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public int DurationSeconds { get; set; }

    public int TrackNumber { get; set; }

    public Guid AlbumId { get; set; }

    public Album Album { get; set; } = null!;
}
