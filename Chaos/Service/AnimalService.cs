using Bogus;
using Chaos.Api.Enums;
using Chaos.Api.Interface;
using Chaos.Api.ResponseEntity;
using Chaos.Infraestructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Threading.Tasks.Dataflow;

namespace Chaos.Api.Service
{
    /// <summary>
    /// Handles all animal-related business logic including purchasing, selling, creation, and pricing.
    /// </summary>
    /// <remarks>
    /// Injects the required services and database context via dependency injection.
    /// </remarks>
    public class AnimalService(
        IUserService userService,
        CasinoDBContext dbContext) : IAnimalService
    {
        /// <summary>
        /// In-memory cache that stores the calculated price for each animal type.
        /// </summary>
        private readonly ConcurrentDictionary<string, double> prices = new();
        private readonly IUserService _userService = userService;
        private readonly CasinoDBContext _dbContext = dbContext;
        private Dictionary<string, AnimalValueConfig>? _configCache;
        public static readonly Random _random = new();
        public static readonly Faker _faker = new();


        #region CACHE CONFIG
        private async Task EnsureConfigLoaded()
        {
            if (_configCache == null)
            {
                _configCache = await _dbContext.AnimalValueConfigs
                    .ToDictionaryAsync(x => x.AnimalType);
            }
        }
        #endregion

        #region BuyAnimal
        /// <summary>
        /// Allows a user to purchase an animal from the shop by validating ownership, availability, and wallet balance.
        /// Returns a success flag and a descriptive message indicating the result of the operation.
        /// </summary>
        /// <param name="animalId">The unique identifier of the animal to purchase.</param>
        /// <param name="userId">The unique identifier of the user making the purchase.</param>
        /// <param name="Name">Optional custom name to assign to the animal after purchase.</param>
        /// <returns>A tuple containing a success flag and a message describing the result.</returns>

        public async Task<(bool success, string message)> BuyAnimal(Guid animalId, Guid userId, string? Name)
        {
            await EnsurePricesLoaded();

            var shopAnimal = await _dbContext.ShopAnimalListings.FirstOrDefaultAsync(a => a.AnimalId == animalId);
            var animal = await GetAnimalById(animalId);
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (animal == null || userId == Guid.Empty || shopAnimal == null)
                return (false, "El animal no existe.");

            bool canBuy = animal.OwnerId == null && shopAnimal.IsSold == false;
            if (!canBuy)
                return (false, "El animal ya está vendido.");

            if (!prices.TryGetValue(animal.AnimalType, out var animalValue))
                return (false, "No hay precio.");

            if (!string.IsNullOrWhiteSpace(Name))
                animal.Name = Name;

            if ((double)user!.Wallet < animalValue)
                return (false, "No tienes dinero.");

            user.Wallet -= (int)animalValue;
            shopAnimal.IsSold = true;
            shopAnimal.SoldAt = DateTime.UtcNow.ToString();
            animal.OwnerId = user.Id;

            await _dbContext.SaveChangesAsync();
            await UpdateAnimalShop();

            return (true, "Animal comprado");
        }

        #endregion

        #region SellAnimal
        /// <summary>
        /// Sells an animal owned by the given user, removes it from the system, and returns its estimated value.
        /// Returns null and an error message if the animal does not exist or does not belong to the user.
        /// </summary>
        /// <param name="id">The unique identifier of the animal to sell.</param>
        /// <param name="userId">The unique identifier of the user attempting to sell the animal.</param>
        /// <returns>A tuple containing the animal's estimated value and an error message if applicable.</returns>
        public async Task<(decimal? value, string error)> SellAnimal(Guid id, Guid userId)
        {
            var animal = await _dbContext.Animals.FirstOrDefaultAsync(a => a.Id == id);

            if (animal == null)
                return (null, "El animal no existe.");

            if (animal.OwnerId != userId)
                return (null, "Este animal no es tuyo...");

            var value = animal.EstimatedValue;
            await RemoveAnimal(animal.Id);
            await _dbContext.SaveChangesAsync();
            return (value, null);
        }
        #endregion

        #region CreateInitialAnimals
        /// <summary>
        /// Seeds the shop with an initial batch of 5 randomly generated animals assigned to the given user.
        /// Should only be called once during initial setup to avoid duplicate listings.
        /// </summary>
        /// <param name="id">The unique identifier of the user to assign the initial animals to.</param>
        /// <returns>A list of shop listings created for the initial animals.</returns>
        public async Task<List<ShopAnimalListing>> CreateInitialAnimals(Guid id)
        {
            var shop = await GetShop();
            var result = new List<ShopAnimalListing>();
            await CalculatePrice();

            for (int i = 0; i < 5; i++)
            {
                var animal = await CreateRandomAnimal(id);

                var listing = new ShopAnimalListing
                {
                    Id = Guid.NewGuid(),
                    ListingPrice = (decimal)prices[animal.AnimalType],
                    IsSold = true,
                    ListedAt = DateTime.UtcNow.ToString(),
                    SoldAt = null,
                    AnimalId = animal.Id,
                    AnimalShopId = shop.Id,
                };

                _dbContext.ShopAnimalListings.Add(listing);
                result.Add(listing);
            }
            await _dbContext.SaveChangesAsync();
            await UpdateAnimalShop();

            return result;
        }
        #endregion

        #region GetAnimals
        /// <summary>
        /// Retrieves all animals currently marked as available in the database.
        /// Maps each database entity to an AnimalResponse DTO before returning the list.
        /// </summary>
        /// <returns>A list of all available animals as AnimalResponse objects.</returns>
        public async Task<List<AnimalResponse>> GetAnimals()
        {
            List<AnimalResponse> animalResponse = [];
            var animals = await _dbContext.Animals.Where(a => a.IsAvailable).ToListAsync();
            foreach (var animal in animals)
            {
                var newAnimal = new AnimalResponse
                {
                    Id = animal.Id,
                    Name = animal.Name,
                    TypeAnimal = animal.AnimalType,
                    Age = (int)animal.Age!,
                    Weight = (int)animal.Weight!,
                    Height = (int)animal.Height!,
                    Health = (HealthEnum)animal.Health,
                    Value = (int)animal.EstimatedValue,
                    OwnerId = animal.OwnerId,

                };
                animalResponse.Add(newAnimal);
            }
            return animalResponse;
        }
        #endregion

        #region GetAnimalById
        /// <summary>
        /// Fetches a single animal from the database by its unique ID.
        /// Returns null if no matching animal is found.
        /// </summary>
        /// <param name="Id">The unique identifier of the animal to retrieve.</param>
        /// <returns>The matching animal as an AnimalResponse, or null if not found.</returns>
        public async Task<Animal> GetAnimalById(Guid Id)
        {
            var animal = await _dbContext.Animals.FirstOrDefaultAsync(a => a.Id == Id);

            if (animal == null)
            {
                return null!;
            }
            return animal;
        }
        #endregion

        #region CreateRandomAnimal
        /// <summary>
        /// Generates a new animal with a random type, health, name, and stats, then persists it to the database.
        /// Automatically calculates the animal's estimated value and rarity before saving.
        /// </summary>
        /// <param name="UserId">The optional owner ID to assign to the newly created animal.</param>
        /// <returns>The newly created and persisted Animal entity.</returns>

        public async Task<Animal> CreateRandomAnimal(Guid? UserId)
        {
            await EnsureConfigLoaded();

            var types = _configCache!.Keys.ToList();
            var typeAnimal = types[_random.Next(types.Count)];

            var healthValues = Enum.GetValues<HealthEnum>();
            var health = healthValues[_random.Next(healthValues.Length)];

            var animal = new Animal
            {
                Id = Guid.NewGuid(),
                Name = _faker.Name.FirstName(),
                AnimalType = typeAnimal,
                Health = (int)health,
                OwnerId = UserId,
                CreatedAt = DateTime.UtcNow.ToString(),
                IsAvailable = true
            };

            await CalculateAnimalRandomStats(animal);

            animal.EstimatedValue = (int)await CalculateValue(animal);
            animal.Rarity = GetRarity(animal);

            _dbContext.Animals.Add(animal);
            await _dbContext.SaveChangesAsync();

            return animal;
        }

        #endregion

        #region RemoveAnimal
        /// <summary>
        /// Marks an animal as unavailable instead of deleting it, preserving historical data.
        /// Persists the change to the database after updating the availability flag.
        /// </summary>
        /// <param name="Id">The unique identifier of the animal to remove.</param>
        public async Task RemoveAnimal(Guid Id)
        {
            var animal = await GetAnimalById(Id);
            animal.IsAvailable = false;
            await _dbContext.SaveChangesAsync();
        }
        #endregion

        #region CalculateValue
        /// <summary>
        /// Computes an animal's monetary value based on its weight, height, age, and health stat.
        /// Doubles the final value if the animal is marked as rare.
        /// </summary>
        /// <param name="animal">The animal entity whose value will be calculated.</param>
        /// <returns>The calculated monetary value of the animal as a double.</returns>
        public Task<double> CalculateValue(Animal animal)
        {
            // ─────────────────────────────────────────────
            // Use the same linear formula as the shop average price:
            // Value = (Weight * 10 + Height * 5 - Age * 2) * Health / 3
            // ─────────────────────────────────────────────

            double baseValue = ((double)animal.Weight * 10 + (double)animal.Height * 5 - (double)animal.Age * 2) * (double)animal.Health / 3.0;


            // Rarity: bonus of 100% (x2)
            if (animal.Rarity)
                baseValue *= 2.0;

            return Task.FromResult(Math.Max(1.0, Math.Round(baseValue, 2)));
        }

        #endregion

        #region CalculateAnimalRandomStats
        /// <summary>
        /// Looks up the stat boundaries for the given animal type and assigns randomized age, weight, and height.
        /// The generated values always fall within the configured min/max range stored in the database.
        /// </summary>
        /// <param name="animal">The animal entity to update with randomized stats.</param>
        /// <returns>The same animal entity with its age, weight, and height populated.</returns>

        public async Task<Animal> CalculateAnimalRandomStats(Animal animal)
        {
            await EnsureConfigLoaded();

            var values = _configCache![animal.AnimalType];

            animal.Age = (int)_random.NextInt64(values.MinAge, values.MaxAge);
            animal.Weight = _random.NextInt64((int)values.MinWeight, (int)values.MaxWeight);
            animal.Height = (int)_random.NextInt64((int)values.MinHeight, (int)values.MaxHeight);

            return animal;
        }

        #endregion

        #region UpdateAnimalShop
        /// <summary>
        /// Ensures the shop always has at least 50 unsold listings by generating new animals to fill any gaps.
        /// Newly created animals are automatically priced and linked to the active shop.
        /// </summary>

        public async Task UpdateAnimalShop()
        {
            await EnsurePricesLoaded();
            await EnsureConfigLoaded();

            var shop = await GetShop();

            var listings = await _dbContext.ShopAnimalListings
                .Where(s => !s.IsSold)
                .ToListAsync();

            if (listings.Count < 50)
            {
                for (int i = listings.Count; i < 50; i++)
                {
                    var animal = await CreateRandomAnimal(null);

                    _dbContext.ShopAnimalListings.Add(new ShopAnimalListing
                    {
                        Id = Guid.NewGuid(),
                        ListingPrice = (decimal)prices[animal.AnimalType],
                        IsSold = false,
                        ListedAt = DateTime.UtcNow.ToString(),
                        AnimalId = animal.Id,
                        AnimalShopId = shop.Id
                    });
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        #endregion

        #region CalculateAvg
        /// <summary>
        /// Calculates the average value of an animal type by averaging the best-case and worst-case stat combinations.
        /// Uses the highest and lowest health values alongside the configured stat boundaries for the type.
        /// </summary>
        /// <param name="type">The animal type to calculate the average value for.</param>
        /// <returns>The average calculated value between the best and worst possible animal of that type.</returns>

        private double CalculateAvg(AnimalValueConfig data)
        {
            var healthValues = Enum.GetValues<HealthEnum>();

            var maxValue = ((double)data.MaxWeight * 10 + (double)data.MaxHeight * 5 - (double)data.MaxAge * 2) * (double)healthValues[0] / 3;
            var minValue = ((double)data.MinWeight * 10 + (double)data.MinHeight * 5 - data.MinAge * 2) * (double)healthValues[5] / 3;

            return (maxValue + minValue) / 2;
        }

        #endregion

        #region CalculatePrice
        /// <summary>
        /// Iterates over every animal type and stores its average calculated price in the in-memory prices dictionary.
        /// Results are rounded to the nearest whole number before being cached.
        /// </summary>

        public async Task CalculatePrice()
        {
            await EnsureConfigLoaded();

            foreach (var config in _configCache!.Values)
            {
                var avg = CalculateAvg(config);
                prices[config.AnimalType] = Math.Round(avg, 0);
            }
        }

        #endregion

        #region GetRarity
        /// <summary>
        /// Determines rarity with a 1-in-4096 chance, similar to a shiny mechanic in classic RPGs.
        /// Returns true if the animal is rare, false otherwise.
        /// </summary>
        /// <param name="animal">The animal entity to evaluate for rarity.</param>
        /// <returns>True if the animal is rare, false otherwise.</returns>
        public bool GetRarity(Animal animal)
        {
            return _random.Next(0, 4096) == 0;
        }
        #endregion

        #region GetAvgValues
        /// <summary>
        /// Returns a snapshot of the current cached prices for all animal types.
        /// Triggers a full price calculation first if the cache is empty.
        /// </summary>
        /// <returns>A concurrent dictionary mapping each animal type to its average price.</returns>
        public async Task<ConcurrentDictionary<string, double>> GetAvgValues()
        {
            await EnsurePricesLoaded();

            return new ConcurrentDictionary<string, double>(prices);
        }
        #endregion

        #region GetShop
        /// <summary>
        /// Retrieves the main animal shop from the database, creating it if it does not yet exist.
        /// The shop is always identified by the fixed name "Yaguete Palace".
        /// </summary>
        /// <returns>The existing or newly created AnimalShop entity.</returns>
        private async Task<AnimalShop> GetShop()
        {
            var Shop = _dbContext.AnimalShops.FirstOrDefault(s => s.ShopName == "Yaguete Palace");
            if (Shop == null)
            {
                Shop = new AnimalShop
                {
                    Id = Guid.NewGuid(),
                    ShopName = "Yaguete Palace",
                    Description = "Best animal shop in Las Vegas",
                    CreatedAt = DateTime.UtcNow.ToString(),
                };
                _dbContext.AnimalShops.Add(Shop);
                await _dbContext.SaveChangesAsync();
            }
            return Shop;
        }
        #endregion

        #region EnsurePricesLoaded
        /// <summary>
        /// Checks whether the prices cache is empty and triggers a full price calculation if so.
        /// Acts as a lazy-loading guard to avoid redundant recalculations on every call.
        /// </summary>
        private async Task EnsurePricesLoaded()
        {
            if (prices.IsEmpty)
            {
                await CalculatePrice();
            }
        }
        #endregion

        #region BuyRandomAnimal

        public async Task<RouletteSpinResult> BuyRandomAnimal(Guid userId)
        {
            var animals = _dbContext.Animals.Where(a => a.OwnerId == null);
            var copy = animals.ToList();
            var candidates = new List<RouletteAnimalDto>(); // 👈 DTO, no AnimalResponse

            if (copy.Count == 0)
                return new RouletteSpinResult
                {
                    Success = false,
                    Message = "No hay animales disponibles",
                    Candidates = new(),
                    Winner = null
                };

            int take = Math.Min(5, copy.Count);
            for (int i = 0; i < take; i++)
            {
                int luck = _random.Next(copy.Count);
                candidates.Add(new RouletteAnimalDto   // 👈 solo Id y Name
                {
                    Id = copy[luck].Id,
                    Name = copy[luck].Name
                });
                copy.RemoveAt(luck);
            }

            // 25% mala suerte
            if (_random.NextDouble() < 0.25)
            {
                return new RouletteSpinResult
                {
                    Success = false,
                    Message = "No has obtenido ningún animal (mala suerte 😢)",
                    Candidates = candidates,
                    Winner = null
                };
            }

            int choosed = _random.Next(candidates.Count);
            var winner = candidates[choosed];

            var user = _userService.GetUserById(userId);
            if (user == null)
                return new RouletteSpinResult
                {
                    Success = false,
                    Message = "Usuario no encontrado",
                    Candidates = candidates,
                    Winner = null
                };

            var elegido = animals.FirstOrDefault(a => a.Id == winner.Id);
            if (elegido == null)
                return new RouletteSpinResult
                {
                    Success = false,
                    Message = "Animal no encontrado en BD",
                    Candidates = candidates,
                    Winner = null
                };

            var shopAnimal = await _dbContext.ShopAnimalListings
                .FirstOrDefaultAsync(a => a.AnimalId == winner.Id);

            if (shopAnimal == null)
                return new RouletteSpinResult
                {
                    Success = false,
                    Message = "Animal no disponible en tienda",
                    Candidates = candidates,
                    Winner = null
                };

            shopAnimal.IsSold = true;
            shopAnimal.SoldAt = DateTime.UtcNow.ToString();
            elegido.OwnerId = user.Id;

            await _dbContext.SaveChangesAsync();

            return new RouletteSpinResult
            {
                Success = true,
                Message = $"¡Has obtenido a {winner.Name}!",
                Candidates = candidates,  // 👈 los 5 para animar la rueda
                Winner = winner       // 👈 el ganador real
            };
        }
        #endregion

        #region RouletteForRoulette

        public async Task<(bool success, string message, List<RouletteSpinResult>? data, int totalSpins)>
    RouletteForRoulette(Guid userId, string dificultad)
        {
            int[] values;
            long price;
            var list = new List<RouletteSpinResult>();

            switch (dificultad)
            {
                case "facil": values = [0, 0, 1, 1, 2, 1, 2]; price = 500000; break;
                case "medio": values = [0, 0, 1, 2, 2, 3, 1, 2, 3, 1]; price = 750000; break;
                case "dificil": values = [0, 0, 1, 2, 3, 4, 2, 3, 4, 1]; price = 1000000; break;
                default: return (false, "Dificultad no válida", null, 0);
            }

            var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return (false, "Usuario no encontrado", null, 0);

            if (user.Wallet < price)
                return (false, "No tienes suficiente dinero", null, 0);

            var random = values[_random.Next(values.Length)];

            for (int i = 0; i < random; i++)
            {
                var result = await BuyRandomAnimal(userId);
                list.Add(result); // ✅ SIEMPRE agrega, aunque sea mala suerte
            }

            user.Wallet -= price;
            await UpdateAnimalShop();

            var anyWinner = list.Any(r => r.Winner != null);

            // ✅ SIEMPRE devuelve list y random, nunca null
            if (!anyWinner)
                return (false, $"Has tenido {random} tiradas y no obtuviste ningún animal 😢", list, random);

            return (true, $"Ruleta completada con {random} tiradas 🎰", list, random);
        }
        #endregion

        #region GetAnimalsByOwnerId

        public async Task<(bool success, string message, List<AnimalResponse>? data)> GetAnimalsByOwnerId(Guid OwnerId)
        {
            var animals = await _dbContext.Animals.Where(a => a.OwnerId == OwnerId && a.IsAvailable).ToListAsync();
            var list = new List<AnimalResponse>();
            if (animals.Count == 0)
            {
                return (false, "No tienes ningun animal...", null);
            }

            foreach (var animal in animals)
            {
                var an = new AnimalResponse()
                {
                    Id = animal.Id,
                    Name = animal.Name,
                    TypeAnimal = animal.AnimalType,
                    Age = (int)animal.Age!,
                    Weight = (int)animal.Weight!,
                    Health = (HealthEnum)animal.Health,
                    Height = (int)animal.Height!,
                    OwnerId = OwnerId,
                    Value = (int)animal.EstimatedValue,
                    IsAvailable = animal.IsAvailable,
                    Rarity = animal.Rarity
                };
                list.Add(an);
            }

            return (true, "Estos son tus animales", list);
        }

        #endregion

        #region GetShopAnimals

        public async Task<(bool success, string message, List<AnimalResponse>? data)> GetShopAnimals()
        {
            // ✅ Join con ShopAnimalListings para garantizar consistencia
            var animals = await _dbContext.Animals
                .Where(a => a.OwnerId == null && a.IsAvailable)
                .Where(a => _dbContext.ShopAnimalListings
                    .Any(s => s.AnimalId == a.Id && !s.IsSold))
                .ToListAsync();

            if (animals.Count == 0)
                return (false, "El casino no tiene ningun animal...", null);

            var list = animals.Select(animal => new AnimalResponse
            {
                Id = animal.Id,
                Name = animal.Name,
                TypeAnimal = animal.AnimalType,
                Age = (int)animal.Age!,
                Weight = (int)animal.Weight!,
                Health = (HealthEnum)animal.Health,
                Height = (int)animal.Height!,
                OwnerId = null,
                Value = (int)animal.EstimatedValue,
                IsAvailable = animal.IsAvailable,
                Rarity = animal.Rarity
            }).ToList();

            return (true, "Estos son los animales del casino", list);
        }

        #endregion
    }
}