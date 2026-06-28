using System.ComponentModel.DataAnnotations;
using Spotflix.Api.Models.Payments;

namespace Spotflix.Api.Dtos.Payments;

public record AddCardDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(150, ErrorMessage = "O nome do titular deve ter no máximo 150 caracteres.")]
    public string HolderName { get; init; } = null!;

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [CreditCard(ErrorMessage = "Número de cartão de crédito inválido.")]
    public string Number { get; init; } = null!;

    [StringLength(40, ErrorMessage = "A bandeira deve ter no máximo 40 caracteres.")]
    public string Brand { get; init; } = "Unknown";

    [Range(0, 1_000_000, ErrorMessage = "O limite deve estar entre 0 e 1.000.000.")]
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
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public Guid CardId { get; init; }

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(150, ErrorMessage = "O estabelecimento deve ter no máximo 150 caracteres.")]
    public string Merchant { get; init; } = null!;

    [Range(0.01, 1_000_000, ErrorMessage = "O valor deve estar entre 0,01 e 1.000.000.")]
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
