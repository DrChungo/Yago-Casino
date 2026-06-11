using Chaos.BlazorCasinoApp.IApiService;

using Chaos.Shared.RequestEntity;
using Chaos.Shared.ResponseEntity;
using System.Net.Http.Json;

namespace Chaos.BlazorCasinoApp.Service
{
    public class BlackjackConfigApiService : IBlackjackConfigService
    {
        private readonly HttpClient _http;
        private readonly string _url;

        public BlackjackConfigApiService(HttpClient http, IConfiguration configuration)
        {
            _http = http;

            string baseUrl = configuration["Api:UrlBase"]
                ?? throw new Exception("No se encontró Api:UrlBase en appsettings.json");

            string endpoint = configuration["Api:Endpoints:UrlBackendAPIBlackjack"]
                ?? throw new Exception("No se encontró Api:Endpoints:UrlBackendAPIBlackjack en appsettings.json");

            _url = baseUrl + endpoint;

            Console.WriteLine("URL cargada para Blackjack Config: " + _url);
        }

        // GET ALL
        public async Task<List<BlackjackResponse>> GetAllAsync()
        {
            var result = await _http.GetFromJsonAsync<List<BlackjackResponse>>(_url);
            return result ?? new List<BlackjackResponse>();
        }

        // GET BY ID
        public async Task<BlackjackResponse> GetByIdAsync(Guid id)
        {
            var result = await _http.GetFromJsonAsync<BlackjackResponse>($"{_url}/{id}");
            return result ?? new BlackjackResponse();
        }

        // CREATE
        public async Task CreateAsync(BlackjackRequest request)
        {
            var response = await _http.PostAsJsonAsync(_url, request);
            response.EnsureSuccessStatusCode();
        }

        // UPDATE
        public async Task UpdateAsync(Guid id, BlackjackRequest request)
        {
            var response = await _http.PutAsJsonAsync($"{_url}/{id}", request);
            response.EnsureSuccessStatusCode();
        }

        // DELETE
        public async Task DeleteAsync(Guid id)
        {
            var response = await _http.DeleteAsync($"{_url}/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException(errorMessage);
            }
        }

        // TOGGLE ACTIVE
        public async Task ToggleActiveAsync(Guid id)
        {
            var response = await _http.PatchAsync($"{_url}/{id}/toggle", null);
            response.EnsureSuccessStatusCode();
        }
    }
}