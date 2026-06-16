using Chaos.Api.Interface;
using Chaos.Infraestructure.Models;
using Microsoft.EntityFrameworkCore;
//test
namespace Chaos.Api.Service
{
    public class ActiveDrinkEffectService : IActiveDrinkEffectService
    {
        private readonly CasinoDBContext _db;

        public ActiveDrinkEffectService(CasinoDBContext db)
        {
            _db = db;
        }

        public async Task<ActiveDrinkEffect?> GetActiveEffectAsync(Guid userId, string effectType)
        {
            return await _db.ActiveDrinkEffects
                .FirstOrDefaultAsync(e => e.UserId == userId
                                       && e.EffectType == effectType
                                       && e.RoundsRemaining > 0);
        }

        public async Task AddEffectAsync(Guid userId, string effectType, int rounds)
        {
            // If effect already exists, stack/refresh rounds
            var existing = await GetActiveEffectAsync(userId, effectType);
            if (existing != null)
            {
                existing.RoundsRemaining += rounds;
                _db.ActiveDrinkEffects.Update(existing);
            }
            else
            {
                _db.ActiveDrinkEffects.Add(new ActiveDrinkEffect
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    EffectType = effectType,
                    RoundsRemaining = rounds,
                    CreatedAt = DateTime.UtcNow.ToString()
                });
            }

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Returns true if the effect was active and a round was consumed.
        /// </summary>
        public async Task<bool> ConsumeRoundAsync(Guid userId, string effectType)
        {
            var effect = await GetActiveEffectAsync(userId, effectType);
            if (effect == null) return false;

            effect.RoundsRemaining--;

            if (effect.RoundsRemaining <= 0)
                _db.ActiveDrinkEffects.Remove(effect);
            else
                _db.ActiveDrinkEffects.Update(effect);

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<ActiveDrinkEffect>> GetAllEffectsForUserAsync(Guid userId)
        {
            return await _db.ActiveDrinkEffects
                .Where(e => e.UserId == userId && e.RoundsRemaining > 0)
                .ToListAsync();
        }

        /// <summary>
        /// Fetches the record with NO RoundsRemaining filter.
        /// Used for WALLET_BOOST_50 where RoundsRemaining = purchase count.
        /// </summary>
        public async Task<ActiveDrinkEffect?> GetPurchaseRecordAsync(Guid userId, string effectType)
        {
            return await _db.ActiveDrinkEffects
                .FirstOrDefaultAsync(e => e.UserId == userId && e.EffectType == effectType);
        }

        /// <summary>
        /// Increments RoundsRemaining by 1 as a purchase counter.
        /// Creates the record on the very first purchase.
        /// </summary>
        public async Task IncrementPurchaseCountAsync(Guid userId, string effectType)
        {
            var record = await GetPurchaseRecordAsync(userId, effectType);

            if (record != null)
            {
                record.RoundsRemaining++;
                _db.ActiveDrinkEffects.Update(record);
            }
            else
            {
                _db.ActiveDrinkEffects.Add(new ActiveDrinkEffect
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    EffectType = effectType,
                    RoundsRemaining = 1,
                    CreatedAt = DateTime.UtcNow.ToString()
                });
            }

            await _db.SaveChangesAsync();
        }
    }
}
