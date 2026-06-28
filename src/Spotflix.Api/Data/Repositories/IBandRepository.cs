using Spotflix.Api.Models.Catalog;

namespace Spotflix.Api.Data.Repositories;

public interface IBandRepository : IRepository<Band>
{
    Task<Band?> GetWithAlbumsAsync(Guid id, CancellationToken ct = default);
}
