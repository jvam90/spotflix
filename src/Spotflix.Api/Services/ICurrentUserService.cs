namespace Spotflix.Api.Services;

/// <summary>Acesso conveniente à identidade do usuário autenticado na requisição atual.</summary>
public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
}
