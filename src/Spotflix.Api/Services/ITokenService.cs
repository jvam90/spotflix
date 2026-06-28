using Spotflix.Api.Dtos.Auth;
using Spotflix.Api.Models;

namespace Spotflix.Api.Services;

public interface ITokenService
{
    /// <summary>Gera access token + refresh token (persistido) para o usuário.</summary>
    Task<TokenResponseDto> CreateTokensAsync(ApplicationUser user, CancellationToken ct = default);

    /// <summary>
    /// Valida e rotaciona um refresh token: revoga o antigo e emite um novo par.
    /// Retorna null se o token for inválido/expirado/revogado.
    /// </summary>
    Task<TokenResponseDto?> RotateRefreshTokenAsync(string refreshToken, CancellationToken ct = default);

    /// <summary>Revoga um refresh token (logout). Retorna false se não encontrado/ativo.</summary>
    Task<bool> RevokeRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
}
