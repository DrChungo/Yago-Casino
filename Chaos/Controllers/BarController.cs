using Chaos.Api.Interface;
using Chaos.Api.RequestEntity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class BarController(IActiveDrinkEffectService drinkEffectService) : Controller
{
    private readonly IActiveDrinkEffectService _drinkEffectService = drinkEffectService;

    private const decimal MultiplyBaseCost = 25_000_000m;

    private static readonly Dictionary<string, int> EffectRounds = new()
    {
        { "GUARANTEED_WIN_1",     1 },
        { "DOUBLE_REWARDS_5",     5 },
        { "PREVENT_LOSS_2",       2 },
        { "WALLET_BOOST_50",      1 }, // rounds ignored — instant effect, no DB row
    };

    // ─────────────────────────────────────────────────────────────
    // NEW: GET /{userId}/wallet-boost-price
    // Derives current price from how many times it was purchased.
    // RoundsRemaining on this record = purchase count.
    // No balance logic here — frontend handles that like the others.
    // ─────────────────────────────────────────────────────────────
    [HttpGet("{userId}/wallet-boost-price")]
    [Authorize]
    public async Task<IActionResult> GetWalletBoostPrice(Guid userId)
    {
        var record = await _drinkEffectService.GetPurchaseRecordAsync(userId, "WALLET_BOOST_50");
        int purchaseCount = record?.RoundsRemaining ?? 0;
        decimal price = MultiplyBaseCost * (decimal)Math.Pow(2, purchaseCount);

        return Ok(new { purchaseCount, currentPrice = price });
    }

    // ─────────────────────────────────────────────────────────────
    // POST /{userId}/drink-effect  ← UNCHANGED from your original
    // ─────────────────────────────────────────────────────────────
    [HttpPost("{userId}/drink-effect")]
    [Authorize]
    public async Task<IActionResult> ActivateDrinkEffect(
        Guid userId,
        [FromBody] ActivateDrinkEffectRequest request)
    {
        if (!EffectRounds.TryGetValue(request.EffectType, out int rounds))
            return BadRequest(new { message = $"Unknown effect type: {request.EffectType}" });

        await _drinkEffectService.AddEffectAsync(userId, request.EffectType, rounds);
        return Ok(new { message = $"Effect {request.EffectType} activated for {rounds} round(s)." });
    }

    // ─────────────────────────────────────────────────────────────
    // GET /{userId}/drink-effects  ← UNCHANGED
    // ─────────────────────────────────────────────────────────────
    [HttpGet("{userId}/drink-effects")]
    [Authorize]
    public async Task<IActionResult> GetActiveEffects(Guid userId)
    {
        var effects = await _drinkEffectService.GetAllEffectsForUserAsync(userId);
        return Ok(effects);
    }
}