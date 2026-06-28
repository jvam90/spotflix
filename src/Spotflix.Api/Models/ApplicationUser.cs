using Microsoft.AspNetCore.Identity;

namespace Spotflix.Api.Models;

/// <summary>
/// Usuário da aplicação. Estende o IdentityUser com chave Guid e campos de domínio.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    public string? FullName { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Refresh tokens emitidos para este usuário.</summary>
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
