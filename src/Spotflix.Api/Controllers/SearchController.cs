using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spotflix.Api.Data.Repositories;
using Spotflix.Api.Dtos.Catalog;

namespace Spotflix.Api.Controllers;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    private readonly ISearchRepository _searchRepository;

    public SearchController(ISearchRepository searchRepository) => _searchRepository = searchRepository;

    /// <summary>
    /// Busca bandas e músicas por um termo (ILIKE '%termo%'), acelerada por índices
    /// GIN trigram. Leitura pública.
    /// </summary>
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<SearchResultDto>> Search(
        [FromQuery] string q,
        [FromQuery] int limit = 20,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Trim().Length < 2)
            return BadRequest(new { message = "Informe ao menos 2 caracteres em 'q'." });

        var bands = await _searchRepository.SearchBandsAsync(q, limit, ct);
        var songs = await _searchRepository.SearchSongsAsync(q, limit, ct);

        var bandDtos = bands.Select(b => new BandDto
        {
            Id = b.Id,
            Name = b.Name,
            Genre = b.Genre,
            FormedYear = b.FormedYear,
            AlbumCount = b.Albums.Count,
        }).ToList();

        var songDtos = songs.Select(s => new SongSearchHitDto
        {
            Id = s.Id,
            Title = s.Title,
            DurationSeconds = s.DurationSeconds,
            AlbumId = s.AlbumId,
            AlbumTitle = s.Album.Title,
            BandId = s.Album.BandId,
            BandName = s.Album.Band.Name,
        }).ToList();

        return Ok(new SearchResultDto { Bands = bandDtos, Songs = songDtos });
    }
}
