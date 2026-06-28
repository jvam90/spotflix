using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spotflix.Api.Authorization;
using Spotflix.Api.Data;
using Spotflix.Api.Dtos.Billing;
using Spotflix.Api.Models.Billing;

namespace Spotflix.Api.Controllers;

[ApiController]
[Route("api/plans")]
public class PlansController : ControllerBase
{
    private readonly AppDbContext _db;

    public PlansController(AppDbContext db) => _db = db;

    /// <summary>Lista os planos disponíveis. Leitura pública (apenas ativos por padrão).</summary>
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PlanDto>>> List([FromQuery] bool includeInactive = false, CancellationToken ct = default)
    {
        var query = _db.Plans.AsNoTracking();
        if (!includeInactive)
            query = query.Where(p => p.IsActive);

        var plans = await query
            .OrderBy(p => p.Price)
            .Select(p => new PlanDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Period = p.Period,
                Description = p.Description,
                IsActive = p.IsActive,
            })
            .ToListAsync(ct);

        return Ok(plans);
    }

    [Authorize(Policy = Policies.AdminOnly)]
    [HttpPost]
    public async Task<ActionResult<PlanDto>> Create(CreatePlanDto dto, CancellationToken ct)
    {
        var plan = new Plan
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Price = dto.Price,
            Period = dto.Period,
            Description = dto.Description,
        };

        _db.Plans.Add(plan);
        await _db.SaveChangesAsync(ct);

        return Ok(new PlanDto
        {
            Id = plan.Id,
            Name = plan.Name,
            Price = plan.Price,
            Period = plan.Period,
            Description = plan.Description,
            IsActive = plan.IsActive,
        });
    }

    [Authorize(Policy = Policies.AdminOnly)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdatePlanDto dto, CancellationToken ct)
    {
        var plan = await _db.Plans.FindAsync([id], ct);
        if (plan is null)
            return NotFound();

        plan.Name = dto.Name;
        plan.Price = dto.Price;
        plan.Period = dto.Period;
        plan.Description = dto.Description;
        plan.IsActive = dto.IsActive;

        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
