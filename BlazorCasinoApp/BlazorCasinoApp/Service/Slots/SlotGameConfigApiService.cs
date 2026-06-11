using Chaos.BlazorCasinoApp.IApiService.Slots;
using Chaos.Shared.RequestEntity;
using Chaos.Shared.RequestEntity.SlotGame;
using Chaos.Shared.ResponseEntity;
using Chaos.Shared.ResponseEntity.SlotGame;
using System.Net.Http.Json;

namespace Chaos.BlazorCasinoApp.Service.Slots
{
    public class SlotGameConfigApiService : ISlotGameService
    {

        private readonly HttpClient _http;
        private readonly string _url;
 public SlotGameConfigApiService(HttpClient http, IConfiguration configuration)
        {
            _http = http;

            string baseUrl = configuration["Api:UrlBase"]
                ?? throw new Exception("No se encontró Api:UrlBase en appsettings.json");

            string endpoint = configuration["Api:Endpoints:UrlBackendAPISlotGameConfig"]
                ?? throw new Exception("No se encontró Api:Endpoints:UrlBackendAPISlotGameConfig en appsettings.json");

            _url = baseUrl + endpoint;

            Console.WriteLine("URL cargada para SlotGameConfig: " + _url);
        }

        // GET ALL
        public async Task<List<SlotGameConfigResponse>> GetAllAsync()
        {
            var result = await _http.GetFromJsonAsync<List<SlotGameConfigResponse>>(_url);
            return result ?? new List<SlotGameConfigResponse>();
        }

        // GET BY ID
        public async Task<SlotGameConfigResponse> GetByIdAsync(Guid id)
        {
            var result = await _http.GetFromJsonAsync<SlotGameConfigResponse>($"{_url}/{id}");
            return result ?? new SlotGameConfigResponse();
        }

        // CREATE
        public async Task CreateAsync(CreateSlotGameConfigRequest request)
        {
            var response = await _http.PostAsJsonAsync($"{_url}/create", request);
            response.EnsureSuccessStatusCode();
        }
        // UPDATE
        public async Task UpdateAsync(Guid id, UpdateSlotGameConfigRequest request)
        {
            var response = await _http.PutAsJsonAsync($"{_url}/{id}", request);
            response.EnsureSuccessStatusCode();
        }

        // DELETE
        public async Task DeleteAsync(Guid id)
        {
            var response = await _http.DeleteAsync($"{_url}/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}

        
    
