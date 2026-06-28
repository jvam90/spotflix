using System.ComponentModel.DataAnnotations;

namespace Spotflix.Api.Dtos.Auth;

public record RegisterDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um endereço de e-mail válido.")]
    public string Email { get; init; } = null!;

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "A senha deve ter entre 8 e 100 caracteres.")]
    public string Password { get; init; } = null!;

    [StringLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
    public string? FullName { get; init; }
}

public record LoginDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um endereço de e-mail válido.")]
    public string Email { get; init; } = null!;

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public string Password { get; init; } = null!;
}

/// <summary>Par de tokens devolvido em login/refresh.</summary>
public record TokenResponseDto
{
    public string AccessToken { get; init; } = null!;
    public DateTime AccessTokenExpiresAt { get; init; }
    public string RefreshToken { get; init; } = null!;
    public DateTime RefreshTokenExpiresAt { get; init; }
}

public record RefreshTokenDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public string RefreshToken { get; init; } = null!;
}

public record ConfirmEmailDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public string UserId { get; init; } = null!;

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public string Token { get; init; } = null!;
}

public record ForgotPasswordDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um endereço de e-mail válido.")]
    public string Email { get; init; } = null!;
}

public record ResetPasswordDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um endereço de e-mail válido.")]
    public string Email { get; init; } = null!;

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public string Token { get; init; } = null!;

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "A senha deve ter entre 8 e 100 caracteres.")]
    public string NewPassword { get; init; } = null!;
}

public record ChangePasswordDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public string CurrentPassword { get; init; } = null!;

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "A nova senha deve ter entre 8 e 100 caracteres.")]
    public string NewPassword { get; init; } = null!;
}
