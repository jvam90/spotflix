using System.ComponentModel.DataAnnotations;
using Spotflix.Api.Models.Payments;

namespace Spotflix.Api.Dtos.Payments;

public record AddCardDto
{
    [Required, StringLength(150)]
    public string HolderName { get; init; } = null!;

    [Required, CreditCard]
    public string Number { get; init; } = null!;

    [StringLength(40)]
    public string Brand { get; init; } = "Unknown";

    [Range(0, 1_000_000)]
    public decimal CreditLimit { get; init; }
}

public record CardDto
{
    public Guid Id { get; init; }
    public string HolderName { get; init; } = null!;
    public string Last4 { get; init; } = null!;
    public string Brand { get; init; } = null!;
    public bool Active { get; init; }
    public decimal AvailableLimit { get; init; }
}

public record AuthorizeTransactionDto
{
    [Required]
    public Guid CardId { get; init; }

    [Required, StringLength(150)]
    public string Merchant { get; init; } = null!;

    [Range(0.01, 1_000_000)]
    public decimal Amount { get; init; }
}

public record TransactionDto
{
    public Guid Id { get; init; }
    public Guid CardId { get; init; }
    public string Merchant { get; init; } = null!;
    public decimal Amount { get; init; }
    public DateTime OccurredAt { get; init; }
    public TransactionStatus Status { get; init; }
    public IReadOnlyList<string> Violations { get; init; } = Array.Empty<string>();
}

public record AuthorizeResultDto
{
    public bool Authorized { get; init; }
    public TransactionDto Transaction { get; init; } = null!;
    public decimal AvailableLimit { get; init; }
}
