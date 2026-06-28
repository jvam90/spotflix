using Spotflix.Api.Models;

namespace Spotflix.Api.Models.Payments;

/// <summary>Cartão (simulado) de um usuário, com limite disponível.</summary>
public class Card
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public ApplicationUser User { get; set; } = null!;

    public string HolderName { get; set; } = null!;

    public string Last4 { get; set; } = null!;

    public string Brand { get; set; } = null!;

    public bool Active { get; set; } = true;

    /// <summary>Limite disponível para autorizações (estado da conta).</summary>
    public decimal AvailableLimit { get; set; }

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
