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
public class AlbumsController : ControllerBase
{
    private readonly AppDbContext _db;

    public AlbumsController(AppDbContext db) => _db = db;

    /// <summary>Lista os álbuns de uma banda. Leitura pública.</summary>
    [AllowAnonymous]
    [HttpGet("bands/{bandId:guid}/albums")]
    public async Task<ActionResult<IReadOnlyList<AlbumDto>>> ListByBand(Guid bandId, CancellationToken ct)
    {
        if (!await _db.Bands.AnyAsync(b => b.Id == bandId, ct))
            return NotFound();

        var albums = await _db.Albums.AsNoTracking()
            .Where(a => a.BandId == bandId)
            .OrderBy(a => a.ReleaseYear)
            .Select(a => new AlbumDto
            {
                Id = a.Id,
                Title = a.Title,
                ReleaseYear = a.ReleaseYear,
                BandId = a.BandId,
                SongCount = a.Songs.Count,
            })
            .ToListAsync(ct);

        return Ok(albums);
    }

    /// <summary>Detalha um álbum e suas músicas. Leitura pública.</summary>
    [AllowAnonymous]
    [HttpGet("albums/{id:guid}")]
    public async Task<ActionResult<AlbumDetailDto>> GetById(Guid id, CancellationToken ct)
    {
        var album = await _db.Albums.AsNoTracking()
            .Where(a => a.Id == id)
            .Select(a => new AlbumDetailDto
            {
                Id = a.Id,
                Title = a.Title,
                ReleaseYear = a.ReleaseYear,
                BandId = a.BandId,
                BandName = a.Band.Name,
                Songs = a.Songs
                    .OrderBy(s => s.TrackNumber)
                    .Select(s => new SongDto
                    {
                        Id = s.Id,
                        Title = s.Title,
                        DurationSeconds = s.DurationSeconds,
                        TrackNumber = s.TrackNumber,
                        AlbumId = s.AlbumId,
                        HasAudio = s.AudioData != null,
                    }).ToList(),
            })
            .FirstOrDefaultAsync(ct);

        return album is null ? NotFound() : Ok(album);
    }

    [Authorize(Policy = Policies.AdminOnly)]
    [HttpPost("bands/{bandId:guid}/albums")]
    public async Task<ActionResult<AlbumDto>> Create(Guid bandId, CreateAlbumDto dto, CancellationToken ct)
    {
        if (!await _db.Bands.AnyAsync(b => b.Id == bandId, ct))
            return NotFound(new { message = "Banda não encontrada." });

        var album = new Album
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            ReleaseYear = dto.ReleaseYear,
            BandId = bandId,
        };

        _db.Albums.Add(album);
        await _db.SaveChangesAsync(ct);

        var result = new AlbumDto { Id = album.Id, Title = album.Title, ReleaseYear = album.ReleaseYear, BandId = bandId, SongCount = 0 };
        return CreatedAtAction(nameof(GetById), new { id = album.Id }, result);
    }

    [Authorize(Policy = Policies.AdminOnly)]
    [HttpPut("albums/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateAlbumDto dto, CancellationToken ct)
    {
        var album = await _db.Albums.FindAsync([id], ct);
        if (album is null)
            return NotFound();

        album.Title = dto.Title;
        album.ReleaseYear = dto.ReleaseYear;

        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [Authorize(Policy = Policies.AdminOnly)]
    [HttpDelete("albums/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var album = await _db.Albums.FindAsync([id], ct);
        if (album is null)
            return NotFound();

        _db.Albums.Remove(album);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
