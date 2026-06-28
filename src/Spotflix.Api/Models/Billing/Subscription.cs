using Spotflix.Api.Models;

namespace Spotflix.Api.Models.Billing;

public enum SubscriptionStatus
{
    Active = 1,
    Canceled = 2,
    Expired = 3,
}

/// <summary>Assinatura de um usuário a um plano.</summary>
public class Subscription
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public ApplicationUser User { get; set; } = null!;

    public Guid PlanId { get; set; }

    public Plan Plan { get; set; } = null!;

    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    public DateTime CurrentPeriodEnd { get; set; }

    public DateTime? CanceledAt { get; set; }
}
