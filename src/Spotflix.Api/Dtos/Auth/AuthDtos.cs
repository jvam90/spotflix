using System.ComponentModel.DataAnnotations;

namespace Spotflix.Api.Dtos.Auth;

public record RegisterDto
{
    [Required, EmailAddress]
    public string Email { get; init; } = null!;

    [Required, StringLength(100, MinimumLength = 8)]
    public string Password { get; init; } = null!;

    [StringLength(150)]
    public string? FullName { get; init; }
}

public record LoginDto
{
    [Required, EmailAddress]
    public string Email { get; init; } = null!;

    [Required]
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
    [Required]
    public string RefreshToken { get; init; } = null!;
}

public record ConfirmEmailDto
{
    [Required]
    public string UserId { get; init; } = null!;

    [Required]
    public string Token { get; init; } = null!;
}

public record ForgotPasswordDto
{
    [Required, EmailAddress]
    public string Email { get; init; } = null!;
}

public record ResetPasswordDto
{
    [Required, EmailAddress]
    public string Email { get; init; } = null!;

    [Required]
    public string Token { get; init; } = null!;

    [Required, StringLength(100, MinimumLength = 8)]
    public string NewPassword { get; init; } = null!;
}

public record ChangePasswordDto
{
    [Required]
    public string CurrentPassword { get; init; } = null!;

    [Required, StringLength(100, MinimumLength = 8)]
    public string NewPassword { get; init; } = null!;
}
