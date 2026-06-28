using System.ComponentModel.DataAnnotations;
using Spotflix.Api.Models.Billing;

namespace Spotflix.Api.Dtos.Billing;

public record PlanDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public decimal Price { get; init; }
    public BillingPeriod Period { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; }
}

public record CreatePlanDto
{
    [Required, StringLength(120)]
    public string Name { get; init; } = null!;

    [Range(0, 100_000)]
    public decimal Price { get; init; }

    [Required]
    public BillingPeriod Period { get; init; }

    public string? Description { get; init; }
}

public record UpdatePlanDto : CreatePlanDto
{
    public bool IsActive { get; init; } = true;
}

public record SubscribeDto
{
    [Required]
    public Guid PlanId { get; init; }

    /// <summary>Cartão a cobrar (obrigatório para planos pagos). Se omitido, usa o primeiro cartão ativo do usuário.</summary>
    public Guid? CardId { get; init; }
}

public record SubscriptionDto
{
    public Guid Id { get; init; }
    public Guid PlanId { get; init; }
    public string PlanName { get; init; } = null!;
    public decimal Price { get; init; }
    public SubscriptionStatus Status { get; init; }
    public DateTime StartedAt { get; init; }
    public DateTime CurrentPeriodEnd { get; init; }
    public DateTime? CanceledAt { get; init; }
}
