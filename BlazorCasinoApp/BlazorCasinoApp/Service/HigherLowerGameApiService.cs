using Chaos.BlazorCasinoApp.IApiService;
using Chaos.Shared.RequestEntity;
using Chaos.Shared.ResponseEntity;
using System.Net.Http.Json;

namespace Chaos.BlazorCasinoApp.Service
{
    public class HigherLowerGameApiService : IHigherLowerGameService
    {
        private readonly HttpClient _http;
        private readonly string _url;

        public HigherLowerGameApiService(HttpClient http, IConfiguration configuration)
        {
            _http = http;

            string baseUrl = configuration["Api:UrlBase"]
                ?? throw new Exception("No se encontró Api:UrlBase en appsettings.json");

            string endpoint = configuration["Api:Endpoints:UrlBackendAPIHigherLowerGame"]
                ?? throw new Exception("No se encontró Api:Endpoints:UrlBackendAPIHigherLowerGame en appsettings.json");

            _url = baseUrl + endpoint;

            Console.WriteLine("URL cargada para obtener Config de Higher Lower Game: " + _url);
        }

        // GET ALL
        public async Task<List<HigherLowerGameResponse>> GetAllAsync()
        {
            var result = await _http.GetFromJsonAsync<List<HigherLowerGameResponse>>(_url);
            return result ?? new List<HigherLowerGameResponse>();
        }

        // GET BY ID
        public async Task<HigherLowerGameResponse?> GetByIdAsync(Guid id)
        {
            var result = await _http.GetFromJsonAsync<HigherLowerGameResponse>($"{_url}/{id}");
            return result ?? new HigherLowerGameResponse();
        }

        // CREATE
        public async Task CreateAsync(HigherLowerGameRequest request)
        {
            var response = await _http.PostAsJsonAsync(_url, request);
            response.EnsureSuccessStatusCode();
        }

        // UPDATE
        public async Task UpdateAsync(Guid id, HigherLowerGameRequest request)
        {
            var response = await _http.PutAsJsonAsync($"{_url}/{id}", request);
            response.EnsureSuccessStatusCode();
        }

        // TOGGLE ACTIVE
        public async Task ToggleActiveAsync(Guid id)
        {
            var response = await _http.PatchAsync($"{_url}/{id}/toggle", null);
            response.EnsureSuccessStatusCode();
        }

        // DELETE
        public async Task DeleteAsync(Guid id)
        {
            var response = await _http.DeleteAsync($"{_url}/{id}");

            if (!response.IsSuccessStatusCode)
            {
                // Lee el mensaje REAL del backend
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException(errorMessage);
            }
        }
    }
}