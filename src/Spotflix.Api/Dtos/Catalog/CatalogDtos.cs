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
    [Required, StringLength(200)]
    public string Name { get; init; } = null!;

    [StringLength(100)]
    public string? Genre { get; init; }

    public string? Bio { get; init; }

    [Range(1900, 2100)]
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
    [Required, StringLength(250)]
    public string Title { get; init; } = null!;

    [Range(1900, 2100)]
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
    [Required, StringLength(300)]
    public string Title { get; init; } = null!;

    [Range(1, 86_400)]
    public int DurationSeconds { get; init; }

    [Range(1, 1000)]
    public int TrackNumber { get; init; }
}

public record UpdateSongDto : CreateSongDto;
