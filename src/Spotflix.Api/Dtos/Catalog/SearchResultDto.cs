namespace Spotflix.Api.Dtos.Catalog;

/// <summary>Resultado combinado de busca por bandas e músicas.</summary>
public record SearchResultDto
{
    public IReadOnlyList<BandDto> Bands { get; init; } = Array.Empty<BandDto>();
    public IReadOnlyList<SongSearchHitDto> Songs { get; init; } = Array.Empty<SongSearchHitDto>();
}

/// <summary>Música encontrada na busca, com contexto de álbum/banda.</summary>
public record SongSearchHitDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = null!;
    public int DurationSeconds { get; init; }
    public Guid AlbumId { get; init; }
    public string AlbumTitle { get; init; } = null!;
    public Guid BandId { get; init; }
    public string BandName { get; init; } = null!;
    public bool HasAudio { get; init; }
}
