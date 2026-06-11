using Chaos.BlazorCasinoApp.IApiService;
using Chaos.Shared.RequestEntity;
using Chaos.Shared.ResponseEntity;
using System.Net.Http.Json;

namespace Chaos.BlazorCasinoApp.Service
{
    public class RouletteGameConfigApiService : IRouletteGameConfigService
    {
        private readonly HttpClient _http;
        private readonly string _url;

        public RouletteGameConfigApiService(HttpClient http, IConfiguration configuration)
        {
            _http = http;

            string baseUrl = configuration["Api:UrlBase"]
                ?? throw new Exception("No se encontró Api:UrlBase en appsettings.json");

            string endpoint = configuration["Api:Endpoints:UrlBackendAPIRouletteGameConfig"]
                ?? throw new Exception("No se encontró Api:Endpoints:UrlBackendAPIRouletteGameConfig en appsettings.json");

            _url = baseUrl + endpoint;
            Console.WriteLine("URL cargada para Roulette Game Config: " + _url);
        }

        public async Task<List<RouletteGameConfigResponse>> GetAllAsync()
        {
            var result = await _http.GetFromJsonAsync<List<RouletteGameConfigResponse>>(_url);
            return result ?? new List<RouletteGameConfigResponse>();
        }

        public async Task<RouletteGameConfigResponse?> GetByIdAsync(Guid id)
        {
            return await _http.GetFromJsonAsync<RouletteGameConfigResponse>($"{_url}/{id}");
        }

        public async Task CreateAsync(RouletteGameConfigRequest request)
        {
            var response = await _http.PostAsJsonAsync(_url, request);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(Guid id, RouletteGameConfigRequest request)
        {
            var response = await _http.PutAsJsonAsync($"{_url}/{id}", request);
            response.EnsureSuccessStatusCode();
        }

        public async Task ToggleActiveAsync(Guid id)
        {
            var response = await _http.PatchAsync($"{_url}/{id}/toggle", null);
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
    }
}
