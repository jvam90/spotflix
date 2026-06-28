using Spotflix.Api.Models;

namespace Spotflix.Api.Data.Repositories;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken?> GetWithUserAsync(string token, CancellationToken ct = default);
}
