using Chaos.Api.Interface.Config;
using Chaos.Api.RequestEntity.Config.AnimalValue;

using Chaos.Api.ResponseEntity.Config.AnimalValue;

using Chaos.Infraestructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Chaos.Api.Service.Config
{
    public class AnimalValueConfigService : IAnimalValueConfigService
    {
        private readonly CasinoDBContext _context;

        public AnimalValueConfigService(CasinoDBContext context)
        {
            _context = context;
        }

        // GET ALL
        public async Task<IEnumerable<AnimalValueResponse>> GetAllAsync()
        {
            return await _context.AnimalValueConfigs
                .Select(x => new AnimalValueResponse
                {
                    Id = x.Id,
                    AnimalType = x.AnimalType,
                    MinAge = x.MinAge,
                    MaxAge = x.MaxAge,
                    MinWeight = x.MinWeight,
                    MaxWeight = x.MaxWeight,
                    MinHeight = x.MinHeight,
                    MaxHeight = x.MaxHeight,
                    MinHealth = x.MinHealth,
                    MaxHealth = x.MaxHealth,
                 
                    Habitat = x.Habitat,
                    IsActive = x.IsActive,
                    UpdatedBy = x.UpdatedBy
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<AnimalImage>> GetAnimalImage()
        {
            return await _context.AnimalValueConfigs
                .Select(x => new AnimalImage
            {
                Id = x.Id,
                AnimalType = x.AnimalType,
                Habitat = x.Habitat,
                ImageUrlNormal = x.ImageUrlNormal,
                ImageUrlMecha = x.ImageUrlMecha
            })
            .ToListAsync();    
        }

        // GET BY ID
        public async Task<AnimalValueResponse?> GetByIdAsync(Guid id)
        {
            var entity = await _context.AnimalValueConfigs
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) return null;

            return new AnimalValueResponse
            {
                Id = entity.Id,
                AnimalType = entity.AnimalType,
                MinAge = entity.MinAge,
                MaxAge = entity.MaxAge,
                MinWeight = entity.MinWeight,
                MaxWeight = entity.MaxWeight,
                MinHeight = entity.MinHeight,
                MaxHeight = entity.MaxHeight,
                MinHealth = entity.MinHealth,
                MaxHealth = entity.MaxHealth,
                ImageUrlMecha = entity.ImageUrlMecha,
                ImageUrlNormal = entity.ImageUrlNormal,
                Habitat = entity.Habitat,
                IsActive = entity.IsActive,
                UpdatedBy = entity.UpdatedBy
            };
        }

        // CREATE
        public async Task<AnimalValueResponse> CreateAsync(CreateAnimalValueConfigRequest request)
        {
            var entity = new AnimalValueConfig
            {
                Id = Guid.NewGuid(),
                AnimalType = request.AnimalType,
                MinAge = request.MinAge,
                MaxAge = request.MaxAge,
                MinWeight = request.MinWeight,
                MaxWeight = request.MaxWeight,
                MinHeight = request.MinHeight,
                MaxHeight = request.MaxHeight,
                MinHealth = request.MinHealth,
                MaxHealth = request.MaxHealth,
                ImageUrlMecha = request.ImageUrlMecha,
                ImageUrlNormal = request.ImageUrlNormal,
                Habitat = request.Habitat,
                IsActive = request.IsActive,
                UpdatedBy = request.UpdatedBy
            };

            _context.AnimalValueConfigs.Add(entity);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(entity.Id)
                ?? throw new Exception("Error al crear la configuración.");
        }

        // UPDATE
        public async Task<AnimalValueResponse?> UpdateAsync(Guid id, UpdateAnimalValueConfigRequest request)
        {
            var entity = await _context.AnimalValueConfigs
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) return null;

            entity.AnimalType = request.AnimalType;
            entity.MinAge = request.MinAge;
            entity.MaxAge = request.MaxAge;
            entity.MinWeight = request.MinWeight;
            entity.MaxWeight = request.MaxWeight;
            entity.MinHeight = request.MinHeight;
            entity.MaxHeight = request.MaxHeight;
            entity.MinHealth = request.MinHealth;
            entity.MaxHealth = request.MaxHealth;
            entity.IsActive = request.IsActive;
            entity.ImageUrlMecha = request.ImageUrlMecha;
            entity.ImageUrlNormal = request.ImageUrlNormal;
            entity.Habitat = request.Habitat;
            entity.UpdatedBy = request.UpdatedBy;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(entity.Id);

        }

        // DELETE
        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _context.AnimalValueConfigs
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) return false;

            _context.AnimalValueConfigs.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}