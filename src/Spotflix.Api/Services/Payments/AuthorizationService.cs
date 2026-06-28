using Spotflix.Api.Data.Repositories;
using Spotflix.Api.Models.Payments;

namespace Spotflix.Api.Services.Payments;

public class AuthorizationService : IPaymentAuthorizationService
{
    private static readonly TimeSpan Window = TimeSpan.FromMinutes(2);
    private const int MaxInWindow = 3;

    private readonly ICardRepository _cardRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ITransactionNotifier _notifier;
    private readonly ILogger<AuthorizationService> _logger;

    public AuthorizationService(
        ICardRepository cardRepository,
        ITransactionRepository transactionRepository,
        ITransactionNotifier notifier,
        ILogger<AuthorizationService> logger)
    {
        _cardRepository = cardRepository;
        _transactionRepository = transactionRepository;
        _notifier = notifier;
        _logger = logger;
    }

    public async Task<AuthorizationResult> AuthorizeAsync(AuthorizationRequest request, CancellationToken ct = default)
    {
        var card = await _cardRepository.GetByIdWithUserAsync(request.CardId, ct);
        if (card is null)
        {
            // Sem cartão não há estado de conta: registramos a tentativa recusada sem vínculo.
            var orphan = NewTransaction(request, TransactionStatus.Declined, new() { Violations.CardNotFound });
            return new AuthorizationResult(orphan, 0m);
        }

        var violations = new List<string>();

        if (!card.Active)
            violations.Add(Violations.CardNotActive);

        // Regras baseadas no histórico autorizado dentro da janela de tempo.
        var windowStart = request.OccurredAt - Window;
        var recent = await _transactionRepository.GetRecentAuthorizedAsync(card.Id, windowStart, ct);

        if (recent.Count >= MaxInWindow)
            violations.Add(Violations.HighFrequencySmallInterval);

        if (recent.Any(t => t.Merchant == request.Merchant && t.Amount == request.Amount))
            violations.Add(Violations.DoubledTransaction);

        var status = violations.Count == 0 ? TransactionStatus.Authorized : TransactionStatus.Declined;
        var transaction = NewTransaction(request, status, violations);
        transaction.CardId = card.Id;

        if (status == TransactionStatus.Authorized)
            _logger.LogInformation("Transaction authorized: Card={CardId}, Amount={Amount}, Merchant={Merchant}", card.Id, request.Amount, request.Merchant);
        else
        {
            _logger.LogWarning("Transaction declined: Card={CardId}, Amount={Amount}, Violations={ViolationCount}", card.Id, request.Amount, violations.Count);
        }

        await _transactionRepository.AddAsync(transaction, ct);
        await _cardRepository.UpdateAsync(card, ct);

        if (status == TransactionStatus.Authorized)
            await _notifier.NotifyAuthorizedAsync(transaction, card.User.Email ?? string.Empty, ct);

        return new AuthorizationResult(transaction, card.AvailableLimit);
    }

    private static Transaction NewTransaction(AuthorizationRequest req, TransactionStatus status, List<string> violations) => new()
    {
        Id = Guid.NewGuid(),
        CardId = req.CardId,
        Merchant = req.Merchant,
        Amount = req.Amount,
        OccurredAt = req.OccurredAt,
        Status = status,
        Violations = violations,
    };
}
