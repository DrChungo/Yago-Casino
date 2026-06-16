using Bogus;
using Chaos.Api.Enums;
using Chaos.Api.Interface;
using Chaos.Api.Models;
using Chaos.Api.ResponseEntity;
using Chaos.Infraestructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace Chaos.Api.Service
{
    /// <summary>
    /// Handles all animal-related business logic including purchasing, selling, creation, and pricing.
    /// </summary>
    /// <remarks>
    /// Injects the required services and database context via dependency injection.
    /// </remarks>
    public class RankingService(
        IUserService userService,
        CasinoDBContext dbContext,
        IAnimalService animalService) : IRankingService
    {
        /// <summary>
        /// In-memory cache that stores the calculated price for each animal type.
        /// </summary>
        private readonly ConcurrentDictionary<string, double> prices = new();
        private readonly IUserService _userService = userService;
        private readonly CasinoDBContext _dbContext = dbContext;
        private readonly IAnimalService _animalService = animalService;
        private Dictionary<string, AnimalValueConfig>? _configCache;
        public static readonly Random _random = new();
        public static readonly Faker _faker = new();

        #region BestThreeAnimals

        public async Task<(bool success, string message, List<AnimalRankingResponse> data)> BestThreeAnimals()
        {
            var bestAnimals = await _dbContext.Animals.OrderByDescending(a => a.EstimatedValue).Take(3).ToListAsync();
            List<AnimalRankingResponse> ranking = [];
            if (bestAnimals.Count == 0) 
            {
                return (false, "No se encontraron aniamles", new List<AnimalRankingResponse>());
            }

            foreach (var animal in bestAnimals)
            {
                var best = new AnimalRankingResponse
                {
                    Name = animal.Name,
                    TypeAnimal = animal.AnimalType,
                    Health = (HealthEnum)animal.Health,
                    OwnerId = animal.OwnerId,
                    Age = (int)animal.Age!,
                    Weight = (int)animal.Weight!,
                    Height = (int)animal.Height!,
                    Value = (int)animal.EstimatedValue,
                    Rarity = animal.Rarity
                };
                ranking.Add(best);
            }
            return (true, "Estos son los mejores animales", ranking);
        }

        #endregion

        #region BestThreeWallets
        public async Task<(bool success, string message, List<UserWalletRankingResponse> data)> BestThreeWallets()
        {
            var bestUsers = await _dbContext.Users.OrderByDescending(a => a.Wallet).Take(3).ToListAsync();
            List<UserWalletRankingResponse> ranking = [];
            if (bestUsers.Count == 0)
            {
                return (false, "No se encontraron los usuarios", new List<UserWalletRankingResponse>());
            }

            foreach (var user in bestUsers)
            {
                var best = new UserWalletRankingResponse
                {
                    Name = user.Name,
                    Wallet = user.Wallet,
                    Mail = user.Email
                };
                ranking.Add(best);
            }
            return (true, "Estos son los mejores usuarios por wallet", ranking);
        }
        #endregion

        #region BestThreeTotalAnimalValues
        public async Task<(bool success, string message, List<UserTotalAnimalValueResponse> data)> BestThreeTotalAnimalValues()
        {

            List<UserTotalAnimalValueResponse> ranking = [];
            var users =  _dbContext.Users.AsNoTracking().Select(user => new UserResponse
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Wallet = user.Wallet,
                    IsAlive = user.IsActive,
                    IsAdmin = user.IsAdmin
                }).ToList();


            foreach (var user in users)
            {
                var animalsResult = await _animalService.GetAnimalsByOwnerId(user.Id);

                if (animalsResult.data == null)
                {
                    continue;
                }

                var totalAnimalValue = animalsResult.data.Sum(a => a.Value);

                var UserTotalAnimalValue = new UserTotalAnimalValueResponse
                {
                    Name = user.Name,
                    TotalAnimalValue = totalAnimalValue,
                    Mail = user.Email,
                };

                ranking.Add(UserTotalAnimalValue);
            }

            ranking = ranking.OrderByDescending(x => x.TotalAnimalValue).Take(3).ToList();

            if (ranking.Count == 0)
            {
                return (false, "No se encontraron animales o usuarios", ranking);
            }

            return (true, "Estos son los mejores usuarios por el total del valor de sus animales", ranking);
        }
        #endregion
    }
}