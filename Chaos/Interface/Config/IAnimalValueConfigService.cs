using Chaos.Api.RequestEntity.Config.AnimalValue;
using Chaos.Api.ResponseEntity.Config.AnimalValue;

namespace Chaos.Api.Interface.Config
{
    public interface IAnimalValueConfigService
    {
        Task<IEnumerable<AnimalValueResponse>> GetAllAsync();
        Task<AnimalValueResponse?> GetByIdAsync(Guid id);
        Task<AnimalValueResponse> CreateAsync(CreateAnimalValueConfigRequest request);
        Task<AnimalValueResponse?> UpdateAsync(Guid id, UpdateAnimalValueConfigRequest request);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<AnimalImage>> GetAnimalImage();
    }
}
