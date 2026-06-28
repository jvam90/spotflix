using Spotflix.Api.Models.Payments;

namespace Spotflix.Api.Services.Payments;

/// <summary>Pedido de autorização: comerciante, quantidade e tempo.</summary>
public record AuthorizationRequest(Guid CardId, string Merchant, decimal Amount, DateTime OccurredAt);

/// <summary>Resultado da autorização, com a transação persistida e o estado resultante.</summary>
public record AuthorizationResult(Transaction Transaction, decimal AvailableLimit)
{
    public bool Authorized => Transaction.Status == TransactionStatus.Authorized;
    public IReadOnlyList<string> Violations => Transaction.Violations;
}

/// <summary>Motivos de recusa (regras do motor de autorização).</summary>
public static class Violations
{
    public const string CardNotFound = "card-not-found";
    public const string CardNotActive = "card-not-active";
    public const string InsufficientLimit = "insufficient-limit";
    public const string HighFrequencySmallInterval = "high-frequency-small-interval";
    public const string DoubledTransaction = "doubled-transaction";
}
