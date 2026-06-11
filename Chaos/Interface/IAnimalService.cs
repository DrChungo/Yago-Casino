using Chaos.Api.Enums;
using Chaos.Api.ResponseEntity;
using Chaos.Infraestructure.Models;
using System.Collections.Concurrent;

namespace Chaos.Api.Interface
{
    public interface IAnimalService
    {
        Task<List<AnimalResponse>> GetAnimals();

        Task<Animal> GetAnimalById(Guid Id);

        Task RemoveAnimal(Guid Id);

        Task<Animal> CreateRandomAnimal(Guid? Id);

        Task<(bool success, string message)> BuyAnimal(Guid animalId, Guid userId, string? Name);

        Task<(decimal? value, string error)> SellAnimal(Guid id, Guid userId);

        Task<double> CalculateValue(Animal animal);

        Task<List<ShopAnimalListing>> CreateInitialAnimals(Guid Id);

        Task<Animal> CalculateAnimalRandomStats(Animal animal);

        Task UpdateAnimalShop();

        Task CalculatePrice();

        bool GetRarity(Animal animal);
        Task<ConcurrentDictionary<string, double>> GetAvgValues();
        Task<(bool success, string message, List<RouletteSpinResult>? data, int totalSpins)> RouletteForRoulette(Guid userId, string dificultad);
        Task<RouletteSpinResult> BuyRandomAnimal(Guid userId);
        Task<(bool success, string message, List<AnimalResponse>? data)> GetAnimalsByOwnerId(Guid OwnerId);
        Task<(bool success, string message, List<AnimalResponse>? data)> GetShopAnimals();

    }
}