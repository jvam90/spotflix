namespace Spotflix.Api.Models.Payments;

public enum TransactionStatus
{
    Authorized = 1,
    Declined = 2,
}

/// <summary>Tentativa de transação em um cartão para um comerciante/valor/instante.</summary>
public class Transaction
{
    public Guid Id { get; set; }

    public Guid CardId { get; set; }

    public Card Card { get; set; } = null!;

    public string Merchant { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateTime OccurredAt { get; set; }

    public TransactionStatus Status { get; set; }

    /// <summary>Motivos de recusa (vazio quando autorizada). Mapeado como text[] no PostgreSQL.</summary>
    public List<string> Violations { get; set; } = new();
}
