
using Chaos.Shared.RequestEntity.AnimalValue;
using Chaos.Shared.ResponseEntity;



namespace Chaos.BlazorCasinoApp.IApiService
{
    public interface IAnimalValueConfigService
    {
        Task<List<AnimalValueResponse>> GetAllAsync();
        Task<AnimalValueResponse?> GetByIdAsync(Guid id);
        Task<AnimalValueResponse> CreateAsync(CreateAnimalValueRequest request);
        Task<AnimalValueResponse?> UpdateAsync(Guid id, UpdateAnimalValueRequest request); // ← Task<AnimalValueResponse?>
        Task<bool> DeleteAsync(Guid id);
        Task<List<string>> GetSvgFilesAsync();
    }
}
