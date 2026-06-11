using Chaos.Infraestructure.Models;

namespace Chaos.Api.Interface
{
    public interface IActiveDrinkEffectService
    {
        Task<ActiveDrinkEffect?> GetActiveEffectAsync(Guid userId, string effectType);
        Task AddEffectAsync(Guid userId, string effectType, int rounds);
        Task<bool> ConsumeRoundAsync(Guid userId, string effectType);
        Task<List<ActiveDrinkEffect>> GetAllEffectsForUserAsync(Guid userId);
        Task<ActiveDrinkEffect?> GetPurchaseRecordAsync(Guid userId, string effectType);
        Task IncrementPurchaseCountAsync(Guid userId, string effectType);
    }
}
