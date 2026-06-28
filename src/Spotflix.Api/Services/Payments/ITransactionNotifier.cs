using Spotflix.Api.Models.Payments;

namespace Spotflix.Api.Services.Payments;

public interface ITransactionNotifier
{
    /// <summary>Avisa o dono do cartão e o comerciante sobre uma transação autorizada.</summary>
    Task NotifyAuthorizedAsync(Transaction transaction, string cardholderEmail, CancellationToken ct = default);
}
