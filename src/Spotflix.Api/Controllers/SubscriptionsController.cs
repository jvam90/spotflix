using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spotflix.Api.Data;
using Spotflix.Api.Dtos.Billing;
using Spotflix.Api.Models.Billing;
using Spotflix.Api.Models.Payments;
using Spotflix.Api.Services;
using Spotflix.Api.Services.Payments;

namespace Spotflix.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/subscriptions")]
public class SubscriptionsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IPaymentAuthorizationService _authorizer;

    public SubscriptionsController(
        AppDbContext db,
        ICurrentUserService currentUser,
        IPaymentAuthorizationService authorizer)
    {
        _db = db;
        _currentUser = currentUser;
        _authorizer = authorizer;
    }

    /// <summary>Assinatura ativa do usuário logado.</summary>
    [HttpGet("me")]
    public async Task<ActionResult<SubscriptionDto>> Me(CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var sub = await _db.Subscriptions.AsNoTracking()
            .Where(s => s.UserId == userId && s.Status == SubscriptionStatus.Active)
            .OrderByDescending(s => s.StartedAt)
            .Select(s => new SubscriptionDto
            {
                Id = s.Id,
                PlanId = s.PlanId,
                PlanName = s.Plan.Name,
                Price = s.Plan.Price,
                Status = s.Status,
                StartedAt = s.StartedAt,
                CurrentPeriodEnd = s.CurrentPeriodEnd,
                CanceledAt = s.CanceledAt,
            })
            .FirstOrDefaultAsync(ct);

        return sub is null ? NotFound(new { message = "Sem assinatura ativa." }) : Ok(sub);
    }

    /// <summary>Assina um plano. Substitui a assinatura ativa anterior, se houver.</summary>
    [HttpPost]
    public async Task<ActionResult<SubscriptionDto>> Subscribe(SubscribeDto dto, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;

        var plan = await _db.Plans.FirstOrDefaultAsync(p => p.Id == dto.PlanId && p.IsActive, ct);
        if (plan is null)
            return BadRequest(new { message = "Plano inválido ou inativo." });

        // Planos pagos passam pelo motor de autorização (pagamento mockado).
        // Planos gratuitos (preço 0) dispensam cartão e ativam direto.
        if (plan.Price > 0)
        {
            var card = dto.CardId is { } cardId
                ? await _db.Cards.FirstOrDefaultAsync(c => c.Id == cardId && c.UserId == userId, ct)
                : await _db.Cards.Where(c => c.UserId == userId && c.Active).OrderBy(c => c.Id).FirstOrDefaultAsync(ct);

            if (card is null)
                return BadRequest(new { message = "É necessário um cartão ativo para assinar um plano pago." });

            var auth = await _authorizer.AuthorizeAsync(
                new AuthorizationRequest(card.Id, "Spotflix", plan.Price, DateTime.UtcNow), ct);

            if (!auth.Authorized)
                return StatusCode(StatusCodes.Status402PaymentRequired, new
                {
                    message = "Pagamento recusado.",
                    violations = auth.Violations,
                });
        }

        // Cancela a assinatura ativa anterior (troca de plano).
        var actives = await _db.Subscriptions
            .Where(s => s.UserId == userId && s.Status == SubscriptionStatus.Active)
            .ToListAsync(ct);
        foreach (var active in actives)
        {
            active.Status = SubscriptionStatus.Canceled;
            active.CanceledAt = DateTime.UtcNow;
        }

        var now = DateTime.UtcNow;
        var sub = new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PlanId = plan.Id,
            Plan = plan,
            Status = SubscriptionStatus.Active,
            StartedAt = now,
            CurrentPeriodEnd = plan.Period == BillingPeriod.Yearly ? now.AddYears(1) : now.AddMonths(1),
        };

        _db.Subscriptions.Add(sub);
        await _db.SaveChangesAsync(ct);

        return Ok(ToDto(sub));
    }

    [HttpPost("cancel")]
    public async Task<IActionResult> Cancel(CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var actives = await _db.Subscriptions
            .Where(s => s.UserId == userId && s.Status == SubscriptionStatus.Active)
            .ToListAsync(ct);

        if (actives.Count == 0)
            return NotFound(new { message = "Sem assinatura ativa." });

        foreach (var active in actives)
        {
            active.Status = SubscriptionStatus.Canceled;
            active.CanceledAt = DateTime.UtcNow;
        }
        await _db.SaveChangesAsync(ct);

        return NoContent();
    }

    private static SubscriptionDto ToDto(Subscription s) => new()
    {
        Id = s.Id,
        PlanId = s.PlanId,
        PlanName = s.Plan.Name,
        Price = s.Plan.Price,
        Status = s.Status,
        StartedAt = s.StartedAt,
        CurrentPeriodEnd = s.CurrentPeriodEnd,
        CanceledAt = s.CanceledAt,
    };
}
