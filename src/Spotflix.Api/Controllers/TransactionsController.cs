using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spotflix.Api.Data.Repositories;
using Spotflix.Api.Dtos.Payments;
using Spotflix.Api.Services;
using Spotflix.Api.Services.Payments;

namespace Spotflix.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/transactions")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICardRepository _cardRepository;
    private readonly IPaymentAuthorizationService _authorizer;
    private readonly ICurrentUserService _currentUser;

    public TransactionsController(
        ITransactionRepository transactionRepository,
        ICardRepository cardRepository,
        IPaymentAuthorizationService authorizer,
        ICurrentUserService currentUser)
    {
        _transactionRepository = transactionRepository;
        _cardRepository = cardRepository;
        _authorizer = authorizer;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Tenta autorizar uma transação para um comerciante/valor, no instante atual,
    /// dado o estado da conta (limite) e o histórico autorizado (frequência/duplicidade).
    /// </summary>
    [HttpPost("authorize")]
    public async Task<ActionResult<AuthorizeResultDto>> Authorize(AuthorizeTransactionDto dto, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;

        // Garante que o cartão pertence ao usuário logado.
        var card = await _cardRepository.GetByIdAsync(dto.CardId, ct);
        if (card is null || card.UserId != userId)
            return NotFound(new { message = "Cartão não encontrado." });

        var result = await _authorizer.AuthorizeAsync(
            new AuthorizationRequest(dto.CardId, dto.Merchant, dto.Amount, DateTime.UtcNow), ct);

        var body = new AuthorizeResultDto
        {
            Authorized = result.Authorized,
            AvailableLimit = result.AvailableLimit,
            Transaction = ToDto(result.Transaction),
        };

        // Autorizada → 200; recusada → 402 Payment Required com os motivos.
        return result.Authorized ? Ok(body) : StatusCode(StatusCodes.Status402PaymentRequired, body);
    }

    [HttpGet("/api/cards/{cardId:guid}/transactions")]
    public async Task<ActionResult<IReadOnlyList<TransactionDto>>> History(Guid cardId, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var card = await _cardRepository.GetByIdAsync(cardId, ct);
        if (card is null || card.UserId != userId)
            return NotFound(new { message = "Cartão não encontrado." });

        var items = await _transactionRepository.GetCardHistoryAsync(cardId, ct);

        var dtos = items.Select(t => new TransactionDto
        {
            Id = t.Id,
            CardId = t.CardId,
            Merchant = t.Merchant,
            Amount = t.Amount,
            OccurredAt = t.OccurredAt,
            Status = t.Status,
            Violations = t.Violations,
        }).ToList();

        return Ok(dtos);
    }

    private static TransactionDto ToDto(Models.Payments.Transaction t) => new()
    {
        Id = t.Id,
        CardId = t.CardId,
        Merchant = t.Merchant,
        Amount = t.Amount,
        OccurredAt = t.OccurredAt,
        Status = t.Status,
        Violations = t.Violations,
    };
}
