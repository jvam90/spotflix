using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spotflix.Api.Authorization;
using Spotflix.Api.Data;
using Spotflix.Api.Dtos.Catalog;
using Spotflix.Api.Models.Catalog;

namespace Spotflix.Api.Controllers;

[ApiController]
[Route("api")]
public class SongsController : ControllerBase
{
    private readonly AppDbContext _db;

    public SongsController(AppDbContext db) => _db = db;

    /// <summary>Lista as músicas de um álbum. Leitura pública.</summary>
    [AllowAnonymous]
    [HttpGet("albums/{albumId:guid}/songs")]
    public async Task<ActionResult<IReadOnlyList<SongDto>>> ListByAlbum(Guid albumId, CancellationToken ct)
    {
        if (!await _db.Albums.AnyAsync(a => a.Id == albumId, ct))
            return NotFound();

        var songs = await _db.Songs.AsNoTracking()
            .Where(s => s.AlbumId == albumId)
            .OrderBy(s => s.TrackNumber)
            .Select(s => new SongDto
            {
                Id = s.Id,
                Title = s.Title,
                DurationSeconds = s.DurationSeconds,
                TrackNumber = s.TrackNumber,
                AlbumId = s.AlbumId,
                HasAudio = s.AudioData != null,
            })
            .ToListAsync(ct);

        return Ok(songs);
    }

    [AllowAnonymous]
    [HttpGet("songs/{id:guid}")]
    public async Task<ActionResult<SongDto>> GetById(Guid id, CancellationToken ct)
    {
        var song = await _db.Songs.AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new SongDto
            {
                Id = s.Id,
                Title = s.Title,
                DurationSeconds = s.DurationSeconds,
                TrackNumber = s.TrackNumber,
                AlbumId = s.AlbumId,
                HasAudio = s.AudioData != null,
            })
            .FirstOrDefaultAsync(ct);

        return song is null ? NotFound() : Ok(song);
    }

    [Authorize(Policy = Policies.AdminOnly)]
    [HttpPost("albums/{albumId:guid}/songs")]
    public async Task<ActionResult<SongDto>> Create(Guid albumId, CreateSongDto dto, CancellationToken ct)
    {
        if (!await _db.Albums.AnyAsync(a => a.Id == albumId, ct))
            return NotFound(new { message = "Álbum não encontrado." });

        var song = new Song
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            DurationSeconds = dto.DurationSeconds,
            TrackNumber = dto.TrackNumber,
            AlbumId = albumId,
        };

        _db.Songs.Add(song);
        await _db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetById), new { id = song.Id }, ToDto(song));
    }

    [Authorize(Policy = Policies.AdminOnly)]
    [HttpPut("songs/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateSongDto dto, CancellationToken ct)
    {
        var song = await _db.Songs.FindAsync([id], ct);
        if (song is null)
            return NotFound();

        song.Title = dto.Title;
        song.DurationSeconds = dto.DurationSeconds;
        song.TrackNumber = dto.TrackNumber;

        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [Authorize(Policy = Policies.AdminOnly)]
    [HttpDelete("songs/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var song = await _db.Songs.FindAsync([id], ct);
        if (song is null)
            return NotFound();

        _db.Songs.Remove(song);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [AllowAnonymous]
    [HttpGet("songs/{id:guid}/stream")]
    public async Task<IActionResult> Stream(Guid id, CancellationToken ct)
    {
        var song = await _db.Songs.AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new { s.AudioData, s.ContentType })
            .FirstOrDefaultAsync(ct);

        if (song is null || song.AudioData is null)
            return NotFound();

        return File(song.AudioData, song.ContentType ?? "audio/mpeg",
                    enableRangeProcessing: true);
    }

    private static SongDto ToDto(Song s) => new()
    {
        Id = s.Id,
        Title = s.Title,
        DurationSeconds = s.DurationSeconds,
        TrackNumber = s.TrackNumber,
        AlbumId = s.AlbumId,
        HasAudio = s.AudioData != null,
    };
}
