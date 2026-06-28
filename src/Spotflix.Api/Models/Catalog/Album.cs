namespace Spotflix.Api.Models.Catalog;

/// <summary>Álbum pertencente a uma banda.</summary>
public class Album
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public int? ReleaseYear { get; set; }

    public Guid BandId { get; set; }

    public Band Band { get; set; } = null!;

    public ICollection<Song> Songs { get; set; } = new List<Song>();
}
