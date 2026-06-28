namespace Spotflix.Api.Configuration;

/// <summary>Configurações de emissão/validação de JWT (seção "Jwt" do appsettings/secrets).</summary>
public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;

    /// <summary>Chave simétrica de assinatura (HMAC-SHA256). Mantenha em User Secrets / variável de ambiente.</summary>
    public string Key { get; set; } = null!;

    public int AccessTokenMinutes { get; set; } = 15;
    public int RefreshTokenDays { get; set; } = 7;
}
