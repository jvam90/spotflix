using Microsoft.AspNetCore.Identity;

namespace Spotflix.Api.Models;

/// <summary>
/// Papel (role) da aplicação. Estende o IdentityRole com chave Guid.
/// </summary>
public class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole() { }

    public ApplicationRole(string roleName) : base(roleName) { }

    public string? Description { get; set; }
}
