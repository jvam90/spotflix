using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spotflix.Api.Data;
using Spotflix.Api.Dtos.Payments;
using Spotflix.Api.Models.Payments;
using Spotflix.Api.Services;

namespace Spotflix.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/cards")]
public class CardsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CardsController(AppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CardDto>>> List(CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var cards = await _db.Cards.AsNoTracking()
            .Where(c => c.UserId == userId)
            .Select(c => new CardDto
            {
                Id = c.Id,
                HolderName = c.HolderName,
                Last4 = c.Last4,
                Brand = c.Brand,
                Active = c.Active,
                AvailableLimit = c.AvailableLimit,
            })
            .ToListAsync(ct);

        return Ok(cards);
    }

    [HttpPost]
    public async Task<ActionResult<CardDto>> Add(AddCardDto dto, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var digits = new string(dto.Number.Where(char.IsDigit).ToArray());

        var card = new Card
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            HolderName = dto.HolderName,
            // Armazena apenas os últimos 4 dígitos — nunca o número completo.
            Last4 = digits.Length >= 4 ? digits[^4..] : digits,
            Brand = dto.Brand,
            Active = true,
            AvailableLimit = dto.CreditLimit,
        };

        _db.Cards.Add(card);
        await _db.SaveChangesAsync(ct);

        return Ok(new CardDto
        {
            Id = card.Id,
            HolderName = card.HolderName,
            Last4 = card.Last4,
            Brand = card.Brand,
            Active = card.Active,
            AvailableLimit = card.AvailableLimit,
        });
    }
}
