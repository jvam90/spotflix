using Spotflix.Api.Data;
using Spotflix.Api.Models.Payments;

namespace Spotflix.Api.Services.Payments;

/// <summary>
/// Notifica dono do cartão (via e-mail) e comerciante (via log/registro) quando
/// uma transação é autorizada, persistindo cada aviso para auditoria.
/// </summary>
public class TransactionNotifier : ITransactionNotifier
{
    private readonly AppDbContext _db;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<TransactionNotifier> _logger;

    public TransactionNotifier(AppDbContext db, IEmailSender emailSender, ILogger<TransactionNotifier> logger)
    {
        _db = db;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task NotifyAuthorizedAsync(Transaction transaction, string cardholderEmail, CancellationToken ct = default)
    {
        var amount = transaction.Amount.ToString("C");

        // Dono do cartão — e-mail (falha não aborta a transação).
        var cardholderMsg = $"Transação de {amount} autorizada em {transaction.Merchant} às {transaction.OccurredAt:u}.";
        try
        {
            await _emailSender.SendAsync(cardholderEmail, "Transação autorizada", cardholderMsg, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao enviar e-mail de notificação para {Email}", cardholderEmail);
        }

        // Comerciante — canal de log (substituível por webhook/integração real).
        var merchantMsg = $"Você recebeu uma transação autorizada de {amount} às {transaction.OccurredAt:u}.";
        _logger.LogInformation("Notificação ao comerciante {Merchant}: {Message}", transaction.Merchant, merchantMsg);

        _db.Notifications.AddRange(
            new Notification
            {
                Id = Guid.NewGuid(),
                TransactionId = transaction.Id,
                RecipientType = NotificationRecipientType.Cardholder,
                Recipient = cardholderEmail,
                Channel = "email",
                Message = cardholderMsg,
            },
            new Notification
            {
                Id = Guid.NewGuid(),
                TransactionId = transaction.Id,
                RecipientType = NotificationRecipientType.Merchant,
                Recipient = transaction.Merchant,
                Channel = "log",
                Message = merchantMsg,
            });

        await _db.SaveChangesAsync(ct);
    }
}
