using Spotflix.Api.Models.Catalog;

namespace Spotflix.Api.Data.Repositories;

public interface ISearchRepository
{
    Task<IReadOnlyList<Band>> SearchBandsAsync(string term, int limit, CancellationToken ct = default);
    Task<IReadOnlyList<Song>> SearchSongsAsync(string term, int limit, CancellationToken ct = default);
}
