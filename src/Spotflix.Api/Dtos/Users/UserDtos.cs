using System.ComponentModel.DataAnnotations;

namespace Spotflix.Api.Dtos.Users;

/// <summary>Representação pública de um usuário (sem dados sensíveis).</summary>
public record UserDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = null!;
    public string? FullName { get; init; }
    public bool EmailConfirmed { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTimeOffset? LockoutEnd { get; init; }
    public bool IsLockedOut { get; init; }
    public IReadOnlyList<string> Roles { get; init; } = Array.Empty<string>();
}

public record UpdateProfileDto
{
    [StringLength(150)]
    public string? FullName { get; init; }
}

public record AssignRolesDto
{
    [Required, MinLength(1)]
    public IReadOnlyList<string> Roles { get; init; } = Array.Empty<string>();
}

public record PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
}
