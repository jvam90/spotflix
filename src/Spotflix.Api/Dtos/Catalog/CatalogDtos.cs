using System.ComponentModel.DataAnnotations;

namespace Spotflix.Api.Dtos.Catalog;

// ---------- Band ----------

public record BandDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Genre { get; init; }
    public int? FormedYear { get; init; }
    public int AlbumCount { get; init; }
}

public record BandDetailDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Genre { get; init; }
    public string? Bio { get; init; }
    public int? FormedYear { get; init; }
    public IReadOnlyList<AlbumDto> Albums { get; init; } = Array.Empty<AlbumDto>();
}

public record CreateBandDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(200, ErrorMessage = "O nome deve ter no máximo 200 caracteres.")]
    public string Name { get; init; } = null!;

    [StringLength(100, ErrorMessage = "O gênero deve ter no máximo 100 caracteres.")]
    public string? Genre { get; init; }

    public string? Bio { get; init; }

    [Range(1900, 2100, ErrorMessage = "O ano de formação deve estar entre 1900 e 2100.")]
    public int? FormedYear { get; init; }
}

public record UpdateBandDto : CreateBandDto;

// ---------- Album ----------

public record AlbumDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = null!;
    public int? ReleaseYear { get; init; }
    public Guid BandId { get; init; }
    public int SongCount { get; init; }
}

public record AlbumDetailDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = null!;
    public int? ReleaseYear { get; init; }
    public Guid BandId { get; init; }
    public string BandName { get; init; } = null!;
    public IReadOnlyList<SongDto> Songs { get; init; } = Array.Empty<SongDto>();
}

public record CreateAlbumDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(250, ErrorMessage = "O título deve ter no máximo 250 caracteres.")]
    public string Title { get; init; } = null!;

    [Range(1900, 2100, ErrorMessage = "O ano de lançamento deve estar entre 1900 e 2100.")]
    public int? ReleaseYear { get; init; }
}

public record UpdateAlbumDto : CreateAlbumDto;

// ---------- Song ----------

public record SongDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = null!;
    public int DurationSeconds { get; init; }
    public int TrackNumber { get; init; }
    public Guid AlbumId { get; init; }
}

public record CreateSongDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(300, ErrorMessage = "O título deve ter no máximo 300 caracteres.")]
    public string Title { get; init; } = null!;

    [Range(1, 86_400, ErrorMessage = "A duração deve estar entre 1 e 86.400 segundos.")]
    public int DurationSeconds { get; init; }

    [Range(1, 1000, ErrorMessage = "O número da faixa deve estar entre 1 e 1000.")]
    public int TrackNumber { get; init; }
}

public record UpdateSongDto : CreateSongDto;
