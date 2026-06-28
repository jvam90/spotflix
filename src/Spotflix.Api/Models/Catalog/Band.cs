namespace Spotflix.Api.Models.Catalog;

/// <summary>Banda/artista do catálogo.</summary>
public class Band
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Genre { get; set; }

    public string? Bio { get; set; }

    public int? FormedYear { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Album> Albums { get; set; } = new List<Album>();
}
