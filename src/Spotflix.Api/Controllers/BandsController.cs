using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spotflix.Api.Authorization;
using Spotflix.Api.Data;
using Spotflix.Api.Dtos.Catalog;
using Spotflix.Api.Dtos.Users;
using Spotflix.Api.Models.Catalog;

namespace Spotflix.Api.Controllers;

[ApiController]
[Route("api/bands")]
public class BandsController : ControllerBase
{
    private readonly AppDbContext _db;

    public BandsController(AppDbContext db) => _db = db;

    /// <summary>Lista bandas (paginado, com busca opcional por nome). Leitura pública.</summary>
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<PagedResult<BandDto>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _db.Bands.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(b => EF.Functions.ILike(b.Name, $"%{term}%"));
        }

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(b => b.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BandDto
            {
                Id = b.Id,
                Name = b.Name,
                Genre = b.Genre,
                FormedYear = b.FormedYear,
                AlbumCount = b.Albums.Count(),
            })
            .ToListAsync(ct);

        return Ok(new PagedResult<BandDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
        });
    }

    /// <summary>Detalha uma banda e seus álbuns. Leitura pública.</summary>
    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BandDetailDto>> GetById(Guid id, CancellationToken ct)
    {
        var band = await _db.Bands.AsNoTracking()
            .Where(b => b.Id == id)
            .Select(b => new BandDetailDto
            {
                Id = b.Id,
                Name = b.Name,
                Genre = b.Genre,
                Bio = b.Bio,
                FormedYear = b.FormedYear,
                Albums = b.Albums
                    .OrderBy(a => a.ReleaseYear)
                    .Select(a => new AlbumDto
                    {
                        Id = a.Id,
                        Title = a.Title,
                        ReleaseYear = a.ReleaseYear,
                        BandId = a.BandId,
                        SongCount = a.Songs.Count(),
                    }).ToList(),
            })
            .FirstOrDefaultAsync(ct);

        return band is null ? NotFound() : Ok(band);
    }

    [Authorize(Policy = Policies.AdminOnly)]
    [HttpPost]
    public async Task<ActionResult<BandDto>> Create(CreateBandDto dto, CancellationToken ct)
    {
        var band = new Band
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Genre = dto.Genre,
            Bio = dto.Bio,
            FormedYear = dto.FormedYear,
        };

        _db.Bands.Add(band);
        await _db.SaveChangesAsync(ct);

        var result = new BandDto { Id = band.Id, Name = band.Name, Genre = band.Genre, FormedYear = band.FormedYear, AlbumCount = 0 };
        return CreatedAtAction(nameof(GetById), new { id = band.Id }, result);
    }

    [Authorize(Policy = Policies.AdminOnly)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateBandDto dto, CancellationToken ct)
    {
        var band = await _db.Bands.FindAsync([id], ct);
        if (band is null)
            return NotFound();

        band.Name = dto.Name;
        band.Genre = dto.Genre;
        band.Bio = dto.Bio;
        band.FormedYear = dto.FormedYear;

        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [Authorize(Policy = Policies.AdminOnly)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var band = await _db.Bands.FindAsync([id], ct);
        if (band is null)
            return NotFound();

        _db.Bands.Remove(band);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
