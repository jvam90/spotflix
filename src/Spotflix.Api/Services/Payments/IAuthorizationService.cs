namespace Spotflix.Api.Services.Payments;

public interface IPaymentAuthorizationService
{
    /// <summary>
    /// Tenta autorizar uma transação dado o estado da conta (cartão/limite) e as
    /// transações já autorizadas (regras de frequência e duplicidade).
    /// </summary>
    Task<AuthorizationResult> AuthorizeAsync(AuthorizationRequest request, CancellationToken ct = default);
}
