using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Spotflix.Api.Configuration;
using Spotflix.Api.Data.Repositories;
using Spotflix.Api.Dtos.Auth;
using Spotflix.Api.Models;

namespace Spotflix.Api.Services;

public class TokenService : ITokenService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtSettings _jwt;
    private readonly ILogger<TokenService> _logger;

    public TokenService(
        IRefreshTokenRepository refreshTokenRepository,
        UserManager<ApplicationUser> userManager,
        IOptions<JwtSettings> jwt,
        ILogger<TokenService> logger)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userManager = userManager;
        _jwt = jwt.Value;
        _logger = logger;
    }

    public async Task<TokenResponseDto> CreateTokensAsync(ApplicationUser user, CancellationToken ct = default)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var (accessToken, accessExpires) = GenerateAccessToken(user, roles);

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = GenerateSecureToken(),
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwt.RefreshTokenDays),
        };

        await _refreshTokenRepository.AddAsync(refreshToken, ct);
        _logger.LogInformation("Tokens created for user {UserId}", user.Id);

        return new TokenResponseDto
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessExpires,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt,
        };
    }

    public async Task<TokenResponseDto?> RotateRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        var existing = await _refreshTokenRepository.GetWithUserAsync(refreshToken, ct);

        if (existing is null || !existing.IsActive)
            return null;

        // Revoga o antigo (rotação) antes de emitir o novo par.
        existing.RevokedAt = DateTime.UtcNow;
        await _refreshTokenRepository.UpdateAsync(existing, ct);

        return await CreateTokensAsync(existing.User, ct);
    }

    public async Task<bool> RevokeRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        var existing = await _refreshTokenRepository.FirstOrDefaultAsync(rt => rt.Token == refreshToken, ct);
        if (existing is null || !existing.IsActive)
        {
            _logger.LogWarning("Attempted to revoke invalid or already revoked token");
            return false;
        }

        existing.RevokedAt = DateTime.UtcNow;
        await _refreshTokenRepository.UpdateAsync(existing, ct);
        _logger.LogInformation("Refresh token revoked for user {UserId}", existing.UserId);
        return true;
    }

    private (string token, DateTime expiresAt) GenerateAccessToken(ApplicationUser user, IList<string> roles)
    {
        var expires = DateTime.UtcNow.AddMinutes(_jwt.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }

    private static string GenerateSecureToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }
}
