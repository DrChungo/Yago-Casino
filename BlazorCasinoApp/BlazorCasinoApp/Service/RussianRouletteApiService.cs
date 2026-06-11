using Chaos.BlazorCasinoApp.IApiService;
using Chaos.Shared.RequestEntity.Config.RussianRoulette;
using Chaos.Shared.ResponseEntity;
using System.Net.Http.Json;

namespace Chaos.BlazorCasinoApp.Service
{
    public class RussianRouletteApiService : IRussianRouletteService
    {
        private readonly HttpClient _http;
        private readonly string _url;

        public RussianRouletteApiService(HttpClient http, IConfiguration configuration)
        {
            _http = http;
            string baseUrl = configuration["Api:UrlBase"] ?? throw new Exception("No se encontró ApiBaseUrl en appsettings.json");
            string endpoint = configuration["Api:Endpoints:UrlBackendAPIGameRussianRoulette"] ?? throw new Exception("No se encontró ApiEndpoints:RussianRoulette en appsettings.json");
            _url = baseUrl + endpoint;
            Console.WriteLine("URL cargada para Russian Roulette: " + _url);


        }

        public async Task CreateAsync(RussianRouletteRequest request)
        {

            var response = await _http.PostAsJsonAsync(_url, request);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(Guid id)
        {
            var response = await _http.DeleteAsync($"{_url}/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException(errorMessage);
            }
        }

        public async Task<List<RussianRouletteResponse>> GetAllAsync()
        {
            var result = await _http.GetFromJsonAsync<List<RussianRouletteResponse>>(_url);
            return result ?? new List<RussianRouletteResponse>();
        }

        public async Task<RussianRouletteResponse> GetByIdAsync(Guid id)
        {
            var result = await _http.GetFromJsonAsync<RussianRouletteResponse>($"{_url}/{id}");
            return result ?? new RussianRouletteResponse();
        }

        public async Task ToggleActiveAsync(Guid id)
        {
            var response = await _http.PostAsync($"{_url}/{id}/toggle-active", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(Guid id, RussianRouletteRequest request)
        {
            var response = await _http.PutAsJsonAsync($"{_url}/{id}", request);
            response.EnsureSuccessStatusCode();

        }
    }
}
