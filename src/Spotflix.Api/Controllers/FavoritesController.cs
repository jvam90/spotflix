using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spotflix.Api.Data.Repositories;
using Spotflix.Api.Dtos.Catalog;
using Spotflix.Api.Models.Catalog;
using Spotflix.Api.Services;

namespace Spotflix.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/favorites")]
public class FavoritesController : ControllerBase
{
    private readonly IFavoritesRepository _favoritesRepository;
    private readonly IRepository<Song> _songRepository;
    private readonly IBandRepository _bandRepository;
    private readonly ICurrentUserService _currentUser;

    public FavoritesController(
        IFavoritesRepository favoritesRepository,
        IRepository<Song> songRepository,
        IBandRepository bandRepository,
        ICurrentUserService currentUser)
    {
        _favoritesRepository = favoritesRepository;
        _songRepository = songRepository;
        _bandRepository = bandRepository;
        _currentUser = currentUser;
    }

    // ---------- Músicas ----------

    [HttpGet("songs")]
    public async Task<ActionResult<IReadOnlyList<SongDto>>> ListSongs(CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var favorites = await _favoritesRepository.GetUserFavoriteSongsAsync(userId, ct);

        var dtos = favorites.Select(f => new SongDto
        {
            Id = f.Song.Id,
            Title = f.Song.Title,
            DurationSeconds = f.Song.DurationSeconds,
            TrackNumber = f.Song.TrackNumber,
            AlbumId = f.Song.AlbumId,
        }).ToList();

        return Ok(dtos);
    }

    [HttpPut("songs/{songId:guid}")]
    public async Task<IActionResult> AddSong(Guid songId, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;

        var exists = await _songRepository.ExistsAsync(s => s.Id == songId, ct);
        if (!exists)
            return NotFound(new { message = "Música não encontrada." });

        await _favoritesRepository.AddFavoriteSongAsync(userId, songId, ct);
        return NoContent();
    }

    [HttpDelete("songs/{songId:guid}")]
    public async Task<IActionResult> RemoveSong(Guid songId, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        await _favoritesRepository.RemoveFavoriteSongAsync(userId, songId, ct);
        return NoContent();
    }

    // ---------- Bandas ----------

    [HttpGet("bands")]
    public async Task<ActionResult<IReadOnlyList<BandDto>>> ListBands(CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var favorites = await _favoritesRepository.GetUserFavoriteBandsAsync(userId, ct);

        var dtos = favorites.Select(f => new BandDto
        {
            Id = f.Band.Id,
            Name = f.Band.Name,
            Genre = f.Band.Genre,
            FormedYear = f.Band.FormedYear,
            AlbumCount = f.Band.Albums.Count,
        }).ToList();

        return Ok(dtos);
    }

    [HttpPut("bands/{bandId:guid}")]
    public async Task<IActionResult> AddBand(Guid bandId, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;

        var exists = await _bandRepository.ExistsAsync(b => b.Id == bandId, ct);
        if (!exists)
            return NotFound(new { message = "Banda não encontrada." });

        await _favoritesRepository.AddFavoriteBandAsync(userId, bandId, ct);
        return NoContent();
    }

    [HttpDelete("bands/{bandId:guid}")]
    public async Task<IActionResult> RemoveBand(Guid bandId, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        await _favoritesRepository.RemoveFavoriteBandAsync(userId, bandId, ct);
        return NoContent();
    }
}
