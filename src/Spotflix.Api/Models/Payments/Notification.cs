namespace Spotflix.Api.Models.Payments;

public enum NotificationRecipientType
{
    Cardholder = 1,
    Merchant = 2,
}

/// <summary>Aviso emitido ao autorizar uma transação (para comerciante e dono do cartão).</summary>
public class Notification
{
    public Guid Id { get; set; }

    public Guid TransactionId { get; set; }

    public Transaction Transaction { get; set; } = null!;

    public NotificationRecipientType RecipientType { get; set; }

    /// <summary>Destino do aviso (e-mail do dono do cartão ou identificador do comerciante).</summary>
    public string Recipient { get; set; } = null!;

    public string Channel { get; set; } = null!;

    public string Message { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
