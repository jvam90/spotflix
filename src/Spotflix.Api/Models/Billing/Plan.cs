namespace Spotflix.Api.Models.Billing;

public enum BillingPeriod
{
    Monthly = 1,
    Yearly = 2,
}

/// <summary>Plano de assinatura que o usuário pode escolher para escutar músicas.</summary>
public class Plan
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public BillingPeriod Period { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
